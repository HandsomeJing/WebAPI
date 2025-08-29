<template>
  <div class="products-container">
    <!-- 顶部导航栏 -->
    <header class="header">
      <div class="header-content">
        <div class="logo">
          <h1>经销商平台</h1>
        </div>
        <div class="user-info">
          <span class="welcome">欢迎，{{ userStore.user?.customerName }}</span>
          <el-badge :value="cartCount" class="cart-badge">
            <el-button type="primary" @click="goToCart">
              <el-icon><ShoppingCart /></el-icon>
              购物车
            </el-button>
          </el-badge>
          <el-button type="text" @click="goToProfile">
            <el-icon><User /></el-icon>
            个人中心
          </el-button>
        </div>
      </div>
    </header>

    <div class="main-content">
      <!-- 左侧筛选面板 -->
      <aside class="filters-panel">
        <div class="filter-section">
          <h3>商品分类</h3>
          <div v-loading="loadingCategories" class="category-list">
            <div 
              v-for="category in categories" 
              :key="category.typeNo"
              class="category-item"
              :class="{ active: selectedCategory === category.typeName }"
              @click="selectCategory(category.typeName)"
            >
              {{ category.typeName }}
            </div>
          </div>
        </div>

        <div class="filter-section">
          <h3>商品类型</h3>
          <div v-loading="loadingTypes" class="type-list">
            <div 
              v-for="type in productTypes" 
              :key="type.typeNo"
              class="type-item"
              :class="{ active: selectedType === type.typeNo }"
              @click="selectType(type.typeNo)"
            >
              {{ type.typeName }}
            </div>
          </div>
        </div>
      </aside>

      <!-- 右侧商品区域 -->
      <main class="products-main">
        <!-- 搜索和排序 -->
        <div class="search-section">
          <div class="search-bar">
            <el-input
              v-model="searchText"
              placeholder="搜索商品名称..."
              size="large"
              clearable
              @keyup.enter="searchProducts"
            >
              <template #append>
                <el-button @click="searchProducts">
                  <el-icon><Search /></el-icon>
                </el-button>
              </template>
            </el-input>
          </div>
          
          <div class="sort-controls">
            <el-select v-model="sortField" placeholder="排序方式" @change="loadProducts">
              <el-option label="默认排序" value="Id" />
              <el-option label="按名称" value="ProductName" />
              <el-option label="按价格" value="SalePrice" />
            </el-select>
            <el-button-group>
              <el-button 
                :type="sortOrder === 'Asc' ? 'primary' : ''" 
                @click="changeSortOrder('Asc')"
              >
                升序
              </el-button>
              <el-button 
                :type="sortOrder === 'Desc' ? 'primary' : ''" 
                @click="changeSortOrder('Desc')"
              >
                降序
              </el-button>
            </el-button-group>
          </div>
        </div>

        <!-- 商品列表 -->
        <div class="products-section" v-loading="loadingProducts">
          <div v-if="!loadingProducts && products.length === 0" class="empty-state">
            <el-empty description="没有找到商品">
              <el-button type="primary" @click="resetFilters">重置筛选条件</el-button>
            </el-empty>
          </div>

          <div v-else class="products-grid">
            <div 
              v-for="product in products" 
              :key="product.id"
              class="product-card"
            >
              <div class="product-image">
                <img 
                  :src="getProductImageUrl(product.productPhotoUrl)" 
                  :alt="product.productName"
                  @error="handleImageError"
                />
              </div>
              
              <div class="product-info">
                <h3 class="product-name" :title="product.productName">
                  {{ product.productName }}
                </h3>
                <p class="product-code">编号: {{ product.productNo }}</p>
                <p class="product-type">{{ product.typeName }}</p>
                <p class="product-brand">品牌: {{ product.productPp }}</p>
              </div>
              
              <div class="product-price">
                <span class="current-price">¥{{ product.salePrice }}</span>
                <span v-if="product.basePrice !== product.salePrice" class="original-price">
                  ¥{{ product.basePrice }}
                </span>
              </div>
              
              <div class="product-actions">
                <el-button 
                  type="primary" 
                  size="small"
                  :loading="addingToCart[product.productNo]"
                  @click="addToCart(product)"
                >
                  <el-icon><Plus /></el-icon>
                  加入购物车
                </el-button>
              </div>
            </div>
          </div>

          <!-- 分页 -->
          <div class="pagination-section">
            <el-pagination
              v-model:current-page="currentPage"
              v-model:page-size="pageSize"
              :page-sizes="[12, 24, 48, 96]"
              :total="totalCount"
              layout="total, sizes, prev, pager, next, jumper"
              @size-change="handleSizeChange"
              @current-change="handleCurrentChange"
            />
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useUserStore, useCartStore } from '@/stores'
import { productApi } from '@/api'
import type { ProductDto, BlongTypeDto, ProductTypeDto, OrderType } from '@/types/api'

const router = useRouter()
const userStore = useUserStore()
const cartStore = useCartStore()

// 响应式数据
const loadingProducts = ref(false)
const loadingCategories = ref(false)
const loadingTypes = ref(false)
const products = ref<ProductDto[]>([])
const categories = ref<BlongTypeDto[]>([])
const productTypes = ref<ProductTypeDto[]>([])
const addingToCart = reactive<Record<string, boolean>>({})

// 搜索和筛选条件
const searchText = ref('')
const selectedCategory = ref('')
const selectedType = ref('')
const sortField = ref('Id')
const sortOrder = ref<'Asc' | 'Desc'>('Asc')

// 分页参数
const currentPage = ref(1)
const pageSize = ref(24)
const totalCount = ref(0)

// 计算属性
const cartCount = computed(() => cartStore.cartCount)

// 加载商品分类
const loadCategories = async () => {
  loadingCategories.value = true
  try {
    categories.value = await productApi.getBlongTypes()
  } catch (error) {
    console.error('加载分类失败:', error)
    ElMessage.error('加载分类失败')
  } finally {
    loadingCategories.value = false
  }
}

// 加载商品类型
const loadProductTypes = async (belongTypeName: string) => {
  if (!belongTypeName) {
    productTypes.value = []
    return
  }
  
  loadingTypes.value = true
  try {
    productTypes.value = await productApi.getProductTypes(belongTypeName)
  } catch (error) {
    console.error('加载商品类型失败:', error)
    ElMessage.error('加载商品类型失败')
  } finally {
    loadingTypes.value = false
  }
}

// 加载商品列表
const loadProducts = async () => {
  loadingProducts.value = true
  try {
    const params = {
      searchText: searchText.value || undefined,
      productType: selectedType.value || undefined,
      belongTypeName: selectedCategory.value || '',
      productProps: undefined,
      sort: sortField.value,
      pageIndex: currentPage.value,
      pageSize: pageSize.value,
      orderType: sortOrder.value === 'Asc' ? 0 as OrderType : 1 as OrderType
    }
    
    const result = await productApi.getProducts(params)
    products.value = result
    // 注意：后端API可能需要返回总数，这里暂时使用商品数量
    totalCount.value = result.length
  } catch (error) {
    console.error('加载商品失败:', error)
    ElMessage.error('加载商品失败')
  } finally {
    loadingProducts.value = false
  }
}

// 选择分类
const selectCategory = (categoryName: string) => {
  selectedCategory.value = selectedCategory.value === categoryName ? '' : categoryName
  selectedType.value = '' // 重置类型选择
  currentPage.value = 1
  
  if (selectedCategory.value) {
    loadProductTypes(selectedCategory.value)
  } else {
    productTypes.value = []
  }
  
  loadProducts()
}

// 选择类型
const selectType = (typeNo: string) => {
  selectedType.value = selectedType.value === typeNo ? '' : typeNo
  currentPage.value = 1
  loadProducts()
}

// 搜索商品
const searchProducts = () => {
  currentPage.value = 1
  loadProducts()
}

// 改变排序方式
const changeSortOrder = (order: 'Asc' | 'Desc') => {
  sortOrder.value = order
  loadProducts()
}

// 重置筛选条件
const resetFilters = () => {
  searchText.value = ''
  selectedCategory.value = ''
  selectedType.value = ''
  sortField.value = 'Id'
  sortOrder.value = 'Asc'
  currentPage.value = 1
  productTypes.value = []
  loadProducts()
}

// 分页事件处理
const handleSizeChange = (size: number) => {
  pageSize.value = size
  currentPage.value = 1
  loadProducts()
}

const handleCurrentChange = (page: number) => {
  currentPage.value = page
  loadProducts()
}

// 添加到购物车
const addToCart = async (product: ProductDto) => {
  addingToCart[product.productNo] = true
  try {
    const success = await cartStore.addToCart(product.productNo, 1)
    if (success) {
      ElMessage.success(`${product.productName} 已添加到购物车`)
    }
  } catch (error) {
    console.error('添加到购物车失败:', error)
    ElMessage.error('添加到购物车失败')
  } finally {
    addingToCart[product.productNo] = false
  }
}

// 获取商品图片URL
const getProductImageUrl = (photoUrl?: string) => {
  if (photoUrl && photoUrl.startsWith('/')) {
    return `http://localhost:7032${photoUrl}`
  }
  return photoUrl || '/img/placeholder.png'
}

// 图片加载错误处理
const handleImageError = (e: Event) => {
  const target = e.target as HTMLImageElement
  target.src = '/img/placeholder.png'
}

// 跳转到购物车
const goToCart = () => {
  router.push('/cart')
}

// 跳转到个人中心
const goToProfile = () => {
  router.push('/profile')
}

// 监听分类变化
watch(selectedCategory, (newCategory) => {
  if (newCategory) {
    loadProductTypes(newCategory)
  } else {
    productTypes.value = []
    selectedType.value = ''
  }
})

// 页面初始化
onMounted(async () => {
  await Promise.all([
    loadCategories(),
    loadProducts(),
    cartStore.fetchCartCount()
  ])
})
</script>

<style scoped>
.products-container {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.header {
  background: white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 20px;
  height: 70px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.logo h1 {
  margin: 0;
  color: #333;
  font-size: 24px;
  font-weight: 600;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 20px;
}

.welcome {
  color: #666;
  font-size: 14px;
}

.cart-badge {
  margin-right: 10px;
}

.main-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
  display: flex;
  gap: 20px;
}

.filters-panel {
  width: 240px;
  flex-shrink: 0;
}

.filter-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.filter-section h3 {
  margin: 0 0 15px 0;
  font-size: 16px;
  color: #333;
  border-bottom: 2px solid #409eff;
  padding-bottom: 8px;
}

.category-list,
.type-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.category-item,
.type-item {
  padding: 10px 12px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
  color: #666;
  font-size: 14px;
}

.category-item:hover,
.type-item:hover {
  background-color: #f0f7ff;
  color: #409eff;
}

.category-item.active,
.type-item.active {
  background-color: #409eff;
  color: white;
}

.products-main {
  flex: 1;
  min-width: 0;
}

.search-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  display: flex;
  gap: 20px;
  align-items: center;
}

.search-bar {
  flex: 1;
  max-width: 400px;
}

.sort-controls {
  display: flex;
  gap: 10px;
  align-items: center;
}

.products-section {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.product-card {
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.3s;
  background: white;
}

.product-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

.product-image {
  width: 100%;
  height: 200px;
  background-color: #f8f9fa;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.product-image img {
  max-width: 100%;
  max-height: 100%;
  object-fit: cover;
}

.product-info {
  padding: 15px;
  border-bottom: 1px solid #f0f0f0;
}

.product-name {
  font-size: 16px;
  font-weight: 500;
  margin: 0 0 8px 0;
  color: #333;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.product-code,
.product-type,
.product-brand {
  font-size: 12px;
  color: #666;
  margin: 4px 0;
}

.product-price {
  padding: 10px 15px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.current-price {
  font-size: 18px;
  font-weight: 600;
  color: #f56c6c;
}

.original-price {
  font-size: 14px;
  color: #999;
  text-decoration: line-through;
}

.product-actions {
  padding: 15px;
  text-align: center;
}

.product-actions .el-button {
  width: 100%;
}

.pagination-section {
  display: flex;
  justify-content: center;
  padding-top: 20px;
  border-top: 1px solid #f0f0f0;
}

.empty-state {
  text-align: center;
  padding: 60px 20px;
}

@media (max-width: 1200px) {
  .main-content {
    flex-direction: column;
  }
  
  .filters-panel {
    width: 100%;
  }
  
  .filter-section {
    margin-bottom: 10px;
  }
  
  .category-list,
  .type-list {
    flex-direction: row;
    flex-wrap: wrap;
  }
}

@media (max-width: 768px) {
  .search-section {
    flex-direction: column;
    align-items: stretch;
  }
  
  .search-bar {
    max-width: 100%;
  }
  
  .sort-controls {
    justify-content: center;
  }
  
  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
    gap: 15px;
  }
  
  .user-info {
    flex-direction: column;
    gap: 10px;
  }
  
  .header-content {
    height: auto;
    padding: 15px 20px;
  }
}
</style>
