<template>
  <div class="orders-container">
    <!-- 顶部导航 -->
    <header class="header">
      <div class="header-content">
        <el-button type="text" @click="goBack">
          <el-icon><ArrowLeft /></el-icon>
          返回首页
        </el-button>
        <h2>我的订单</h2>
        <div></div>
      </div>
    </header>

    <div class="orders-content" v-loading="loading">
      <!-- 空状态 -->
      <el-empty v-if="!loading && orders.length === 0" description="暂无订单数据">
        <el-button type="primary" @click="goToProducts">去购物</el-button>
      </el-empty>

      <!-- 订单列表 -->
      <div v-else class="orders-list">
        <div 
          v-for="order in orders" 
          :key="order.id"
          class="order-card"
        >
          <div class="order-header">
            <div class="order-info">
              <span class="order-no">订单号: {{ order.saleOrderNo }}</span>
              <span class="order-date">下单时间: {{ formatDate(order.inputDate) }}</span>
            </div>
            <div class="order-status">
              <el-tag type="success">已提交</el-tag>
            </div>
          </div>

          <div class="order-details">
            <div class="delivery-info">
              <p><strong>交货日期:</strong> {{ formatDate(order.deliveryDate) }}</p>
              <p v-if="order.remark"><strong>备注:</strong> {{ order.remark }}</p>
            </div>
          </div>

          <div class="order-actions">
            <el-button 
              type="primary" 
              size="small"
              @click="viewOrderDetails(order.saleOrderNo)"
            >
              查看详情
            </el-button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import type { SaleOrderDto } from '@/types/api'

const router = useRouter()

const loading = ref(false)
const orders = ref<SaleOrderDto[]>([])

// 模拟订单数据（实际项目中需要调用API）
const loadOrders = async () => {
  loading.value = true
  try {
    // 这里应该调用订单列表API
    // 由于后端没有提供订单列表接口，我们使用模拟数据
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    orders.value = [
      {
        id: 1,
        saleOrderNo: '1a2ccecd-a445-4b57-94f2-0e0e586bd728',
        customerNo: 'AHAQWYZ',
        invoiceNo: '???????',
        inputDate: '2022-07-27T10:57:51.260',
        stockNo: '',
        editUserNo: 'AHAQWYZ',
        deliveryDate: '2022-07-27T10:57:51.260',
        remark: '撒大大'
      },
      {
        id: 2,
        saleOrderNo: '7bf2dd35-a230-4c4a-8a93-044b5ed4245e',
        customerNo: 'AHAQWYZ',
        invoiceNo: '???????',
        inputDate: '2022-07-25T11:46:36.770',
        stockNo: '',
        editUserNo: 'AHAQWYZ',
        deliveryDate: '2022-07-26T00:00:00.000',
        remark: '撒大大'
      }
    ]
  } catch (error) {
    console.error('加载订单失败:', error)
    ElMessage.error('加载订单失败')
  } finally {
    loading.value = false
  }
}

// 格式化日期
const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 查看订单详情
const viewOrderDetails = (orderNo: string) => {
  // 这里可以跳转到订单详情页或显示详情弹窗
  ElMessage.info(`查看订单详情: ${orderNo}`)
}

// 返回首页
const goBack = () => {
  router.push('/products')
}

// 去购物
const goToProducts = () => {
  router.push('/products')
}

// 页面初始化
onMounted(async () => {
  await loadOrders()
})
</script>

<style scoped>
.orders-container {
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

.orders-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.order-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 15px;
  padding-bottom: 15px;
  border-bottom: 1px solid #f0f0f0;
}

.order-info {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.order-no {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.order-date {
  font-size: 14px;
  color: #666;
}

.order-details {
  margin-bottom: 15px;
}

.delivery-info p {
  margin: 8px 0;
  font-size: 14px;
  color: #666;
}

.delivery-info strong {
  color: #333;
}

.order-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

@media (max-width: 768px) {
  .order-header {
    flex-direction: column;
    gap: 10px;
    align-items: flex-start;
  }

  .order-actions {
    justify-content: flex-start;
  }

  .orders-content {
    padding: 15px;
  }
}
</style>
