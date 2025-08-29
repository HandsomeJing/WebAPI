<template>
  <div class="order-confirm-container">
    <!-- 顶部导航 -->
    <header class="header">
      <div class="header-content">
        <el-button type="text" @click="goBack">
          <el-icon><ArrowLeft /></el-icon>
          返回购物车
        </el-button>
        <h2>确认订单</h2>
        <div></div>
      </div>
    </header>

    <div class="order-content" v-loading="loading">
      <!-- 订单商品列表 -->
      <div class="order-items">
        <div class="section-title">订单商品</div>
        <div class="items-list">
          <div 
            v-for="item in orderItems" 
            :key="item.id"
            class="order-item"
          >
            <div class="item-image">
              <img 
                :src="getProductImageUrl(item.productDto?.productPhotoUrl)" 
                :alt="item.productDto?.productName"
                @error="handleImageError"
              />
            </div>
            
            <div class="item-info">
              <h3 class="item-name">{{ item.productDto?.productName }}</h3>
              <p class="item-code">编号: {{ item.productNo }}</p>
              <p class="item-type">{{ item.productDto?.typeName }}</p>
            </div>
            
            <div class="item-price">
              <span class="price">¥{{ item.productDto?.salePrice }}</span>
            </div>
            
            <div class="item-quantity">
              <span>x {{ item.productNum }}</span>
            </div>
            
            <div class="item-total">
              <span class="total-price">
                ¥{{ ((item.productDto?.salePrice || 0) * item.productNum).toFixed(2) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- 订单信息 -->
      <div class="order-info">
        <div class="section-title">订单信息</div>
        <el-form 
          ref="orderFormRef"
          :model="orderForm"
          :rules="orderRules"
          label-width="100px"
          class="order-form"
        >
          <el-form-item label="交货日期" prop="deliveryDate">
            <el-date-picker
              v-model="orderForm.deliveryDate"
              type="date"
              placeholder="选择交货日期"
              :disabled-date="disabledDate"
              style="width: 300px"
            />
          </el-form-item>
          
          <el-form-item label="备注信息">
            <el-input
              v-model="orderForm.remark"
              type="textarea"
              :rows="3"
              placeholder="请输入备注信息（可选）"
              maxlength="200"
              show-word-limit
              style="width: 400px"
            />
          </el-form-item>
        </el-form>
      </div>

      <!-- 价格汇总 -->
      <div class="price-summary">
        <div class="section-title">价格明细</div>
        <div class="summary-content">
          <div class="summary-row">
            <span class="label">商品总价:</span>
            <span class="value">¥{{ totalPrice.toFixed(2) }}</span>
          </div>
          <div class="summary-row">
            <span class="label">运费:</span>
            <span class="value">¥0.00</span>
          </div>
          <div class="summary-row total-row">
            <span class="label">订单总计:</span>
            <span class="value total-amount">¥{{ totalPrice.toFixed(2) }}</span>
          </div>
        </div>
      </div>

      <!-- 提交按钮 -->
      <div class="submit-section">
        <el-button 
          type="primary" 
          size="large"
          :loading="submitting"
          @click="submitOrder"
        >
          {{ submitting ? '提交中...' : '提交订单' }}
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { orderApi } from '@/api'
import type { ShoppingCartDto } from '@/types/api'

const router = useRouter()

const loading = ref(false)
const submitting = ref(false)
const orderItems = ref<ShoppingCartDto[]>([])
const orderFormRef = ref<FormInstance>()

// 订单表单数据
const orderForm = reactive({
  deliveryDate: '',
  remark: ''
})

// 表单验证规则
const orderRules: FormRules = {
  deliveryDate: [
    { required: true, message: '请选择交货日期', trigger: 'change' }
  ]
}

// 计算总价
const totalPrice = computed(() => {
  return orderItems.value.reduce((total, item) => {
    const price = item.productDto?.salePrice || 0
    return total + (price * item.productNum)
  }, 0)
})

// 禁用日期（只能选择今天之后的日期）
const disabledDate = (time: Date) => {
  return time.getTime() < Date.now() - 24 * 60 * 60 * 1000
}

// 加载订单确认数据
const loadOrderData = async () => {
  loading.value = true
  try {
    orderItems.value = await orderApi.getOrderConfirm()
    if (orderItems.value.length === 0) {
      ElMessage.warning('没有选中的商品，请返回购物车重新选择')
      router.push('/cart')
    }
  } catch (error) {
    console.error('加载订单数据失败:', error)
    ElMessage.error('加载订单数据失败')
  } finally {
    loading.value = false
  }
}

// 提交订单
const submitOrder = async () => {
  if (!orderFormRef.value) return
  
  try {
    const valid = await orderFormRef.value.validate()
    if (!valid) return
    
    submitting.value = true
    
    const orderData = {
      deliveryDate: orderForm.deliveryDate,
      remark: orderForm.remark || ''
    }
    
    const success = await orderApi.submitOrder(orderData)
    
    if (success) {
      ElMessage.success('订单提交成功')
      // 跳转到订单列表或订单详情页
      router.push('/orders')
    } else {
      ElMessage.error('订单提交失败')
    }
  } catch (error) {
    console.error('提交订单失败:', error)
    ElMessage.error('订单提交失败，请稍后重试')
  } finally {
    submitting.value = false
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

// 返回购物车
const goBack = () => {
  router.push('/cart')
}

// 页面初始化
onMounted(async () => {
  await loadOrderData()
})
</script>

<style scoped>
.order-confirm-container {
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

.order-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 2px solid #409eff;
}

.order-items,
.order-info,
.price-summary {
  background: white;
  border-radius: 8px;
  padding: 20px;
}

.items-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.order-item {
  padding: 15px;
  border: 1px solid #f0f0f0;
  border-radius: 6px;
  display: flex;
  align-items: center;
  gap: 15px;
}

.item-image {
  width: 60px;
  height: 60px;
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
  font-size: 14px;
  font-weight: 500;
  margin: 0 0 6px 0;
  color: #333;
}

.item-code,
.item-type {
  font-size: 12px;
  color: #666;
  margin: 2px 0;
}

.item-price {
  width: 80px;
  text-align: center;
}

.price {
  font-size: 14px;
  font-weight: 600;
  color: #f56c6c;
}

.item-quantity {
  width: 60px;
  text-align: center;
  font-size: 14px;
  color: #666;
}

.item-total {
  width: 100px;
  text-align: right;
}

.total-price {
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

.order-form {
  max-width: 600px;
}

.summary-content {
  max-width: 400px;
  margin-left: auto;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
}

.summary-row:not(:last-child) {
  border-bottom: 1px solid #f0f0f0;
}

.summary-row.total-row {
  padding-top: 15px;
  margin-top: 10px;
  border-top: 2px solid #f0f0f0;
}

.summary-row .label {
  font-size: 14px;
  color: #666;
}

.summary-row .value {
  font-size: 14px;
  color: #333;
  font-weight: 500;
}

.total-row .label {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.total-amount {
  font-size: 18px !important;
  font-weight: 600 !important;
  color: #f56c6c !important;
}

.submit-section {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

.submit-section .el-button {
  width: 200px;
  height: 50px;
  font-size: 16px;
}

@media (max-width: 768px) {
  .order-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }

  .item-image {
    align-self: center;
  }

  .item-price,
  .item-quantity,
  .item-total {
    width: 100%;
    text-align: left;
  }

  .summary-content {
    max-width: 100%;
    margin-left: 0;
  }

  .order-form {
    max-width: 100%;
  }

  .order-content {
    padding: 15px;
  }
}
</style>
