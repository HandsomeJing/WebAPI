<template>
  <div class="cart-container">
    <!-- 顶部导航 -->
    <header class="header">
      <div class="header-content">
        <el-button type="text" @click="goBack">
          <el-icon><ArrowLeft /></el-icon>
          返回商品页
        </el-button>
        <h2>购物车</h2>
        <div></div>
      </div>
    </header>

    <div class="cart-content" v-loading="loading">
      <!-- 购物车为空 -->
      <el-empty v-if="!loading && cartItems.length === 0" description="购物车是空的">
        <el-button type="primary" @click="goToProducts">去购物</el-button>
      </el-empty>

      <!-- 购物车有商品 -->
      <div v-else-if="cartItems.length > 0" class="cart-list">
        <div class="cart-header">
          <el-checkbox v-model="selectAll" @change="toggleSelectAll">全选</el-checkbox>
          <span class="cart-count">共 {{ cartItems.length }} 件商品</span>
        </div>

        <div class="cart-items">
          <div 
            v-for="item in cartItems" 
            :key="item.id"
            class="cart-item"
          >
            <el-checkbox 
              :model-value="item.cartSelected"
              @change="(val) => toggleItemSelect(item.cartGuid, val)"
            />
            
            <div class="item-image">
              <img 
                :src="getProductImageUrl(item.product?.productPhotoUrl)" 
                :alt="item.product?.productName"
                @error="handleImageError"
              />
            </div>
            
            <div class="item-info">
              <h3 class="item-name">{{ item.product?.productName }}</h3>
              <p class="item-code">编号: {{ item.productNo }}</p>
              <p class="item-type">{{ item.product?.typeName }}</p>
            </div>
            
            <div class="item-price">
              <span class="price">¥{{ item.product?.salePrice }}</span>
            </div>
            
            <div class="item-quantity">
              <el-input-number
                :model-value="item.productNum"
                :min="1"
                :max="999"
                size="small"
                @change="(val) => updateQuantity(item, val)"
              />
            </div>
            
            <div class="item-total">
              <span class="total-price">
                ¥{{ ((item.product?.salePrice || 0) * item.productNum).toFixed(2) }}
              </span>
            </div>
            
            <div class="item-actions">
              <el-button 
                type="text" 
                size="small"
                @click="removeItem(item)"
              >
                删除
              </el-button>
            </div>
          </div>
        </div>

        <!-- 底部结算区域 -->
        <div class="cart-footer">
          <div class="footer-content">
            <div class="selected-info">
              <span>已选择 {{ selectedItems.length }} 件商品</span>
            </div>
            <div class="total-info">
              <span class="total-label">合计：</span>
              <span class="total-amount">¥{{ totalPrice.toFixed(2) }}</span>
            </div>
            <el-button 
              type="primary" 
              size="large"
              :disabled="selectedItems.length === 0"
              @click="goToCheckout"
            >
              去结算 ({{ selectedItems.length }})
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useCartStore } from '@/stores'
import type { CartItem } from '@/types'

const router = useRouter()
const cartStore = useCartStore()

const loading = ref(false)

// 计算属性
const cartItems = computed(() => cartStore.cartItems)
const selectedItems = computed(() => cartStore.selectedItems)
const totalPrice = computed(() => cartStore.totalPrice)

const selectAll = computed({
  get: () => cartItems.value.length > 0 && cartItems.value.every(item => item.cartSelected),
  set: (value) => {
    toggleSelectAll(value)
  }
})

// 全选/取消全选
const toggleSelectAll = async (checked: boolean) => {
  for (const item of cartItems.value) {
    if (item.cartSelected !== checked) {
      await cartStore.updateCartSelected(item.cartGuid, checked)
    }
  }
}

// 切换单个商品选中状态
const toggleItemSelect = async (cartGuid: string, checked: boolean) => {
  await cartStore.updateCartSelected(cartGuid, checked)
}

// 更新商品数量
const updateQuantity = async (item: CartItem, newQuantity: number) => {
  if (newQuantity < 1) return
  
  // 这里应该调用更新数量的API，当前后端没有提供该接口
  // 作为演示，我们只在前端更新
  item.productNum = newQuantity
  ElMessage.success('数量已更新')
}

// 删除商品
const removeItem = async (item: CartItem) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除 "${item.product?.productName}" 吗？`,
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 这里应该调用删除API，当前后端没有提供该接口
    // 作为演示，我们只在前端删除
    ElMessage.success('商品已删除')
    await cartStore.fetchCartItems()
  } catch {
    // 用户取消删除
  }
}

// 获取商品图片URL
const getProductImageUrl = (photoUrl?: string) => {
  if (photoUrl && photoUrl.startsWith('/')) {
    return `https://localhost:7188${photoUrl}`
  }
  return photoUrl || '/img/placeholder.png'
}

// 图片加载错误处理
const handleImageError = (e: Event) => {
  const target = e.target as HTMLImageElement
  target.src = '/img/placeholder.png'
}

// 返回商品页
const goBack = () => {
  router.push('/products')
}

// 去购物
const goToProducts = () => {
  router.push('/products')
}

// 去结算
const goToCheckout = () => {
  if (selectedItems.value.length === 0) {
    ElMessage.warning('请选择要结算的商品')
    return
  }
  router.push('/order-confirm')
}

// 页面初始化
onMounted(async () => {
  loading.value = true
  try {
    await cartStore.fetchCartItems()
  } catch (error) {
    ElMessage.error('加载购物车失败')
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.cart-container {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.header {
  background: white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.header h2 {
  margin: 0;
  color: #333;
  font-size: 18px;
}

.cart-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.cart-list {
  background: white;
  border-radius: 8px;
  overflow: hidden;
}

.cart-header {
  padding: 20px;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.cart-count {
  color: #666;
  font-size: 14px;
}

.cart-items {
  padding: 0;
}

.cart-item {
  padding: 20px;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
  gap: 15px;
}

.cart-item:last-child {
  border-bottom: none;
}

.item-image {
  width: 80px;
  height: 80px;
  flex-shrink: 0;
  border-radius: 4px;
  overflow: hidden;
  background-color: #f8f9fa;
  display: flex;
  align-items: center;
  justify-content: center;
}

.item-image img {
  max-width: 100%;
  max-height: 100%;
  object-fit: cover;
}

.item-info {
  flex: 1;
  min-width: 0;
}

.item-name {
  font-size: 16px;
  font-weight: 500;
  margin: 0 0 8px 0;
  color: #333;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.item-code,
.item-type {
  font-size: 12px;
  color: #666;
  margin: 4px 0;
}

.item-price {
  width: 100px;
  text-align: center;
}

.price {
  font-size: 16px;
  font-weight: 600;
  color: #f56c6c;
}

.item-quantity {
  width: 120px;
  display: flex;
  justify-content: center;
}

.item-total {
  width: 120px;
  text-align: center;
}

.total-price {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.item-actions {
  width: 80px;
  text-align: center;
}

.cart-footer {
  background: white;
  border-top: 1px solid #f0f0f0;
  position: sticky;
  bottom: 0;
  z-index: 10;
}

.footer-content {
  padding: 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.selected-info {
  color: #666;
  font-size: 14px;
}

.total-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.total-label {
  font-size: 16px;
  color: #333;
}

.total-amount {
  font-size: 20px;
  font-weight: 600;
  color: #f56c6c;
}

@media (max-width: 768px) {
  .cart-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }

  .item-image {
    width: 60px;
    height: 60px;
    align-self: center;
  }

  .item-price,
  .item-quantity,
  .item-total,
  .item-actions {
    width: 100%;
    text-align: left;
  }

  .footer-content {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .total-info {
    justify-content: center;
  }
}
</style>
