<template>
  <div class="profile-container">
    <!-- 顶部导航 -->
    <header class="header">
      <div class="header-content">
        <el-button type="text" @click="goBack">
          <el-icon><ArrowLeft /></el-icon>
          返回首页
        </el-button>
        <h2>个人中心</h2>
        <div></div>
      </div>
    </header>

    <div class="profile-content">
      <!-- 用户信息卡片 -->
      <div class="user-info-card">
        <div class="user-avatar">
          <el-avatar :size="80">
            {{ userStore.user?.customerName?.charAt(0) || 'U' }}
          </el-avatar>
        </div>
        <div class="user-details">
          <h3>{{ userStore.user?.customerName }}</h3>
          <p class="user-no">客户编号: {{ userStore.user?.customerNo }}</p>
        </div>
      </div>

      <!-- 功能菜单 -->
      <div class="menu-section">
        <div class="menu-item" @click="goToOrders">
          <div class="menu-icon">
            <el-icon><Document /></el-icon>
          </div>
          <div class="menu-content">
            <span class="menu-title">我的订单</span>
            <span class="menu-desc">查看订单历史</span>
          </div>
          <el-icon class="menu-arrow"><ArrowRight /></el-icon>
        </div>

        <div class="menu-item" @click="goToCart">
          <div class="menu-icon">
            <el-icon><ShoppingCart /></el-icon>
          </div>
          <div class="menu-content">
            <span class="menu-title">购物车</span>
            <span class="menu-desc">{{ cartCount }} 件商品</span>
          </div>
          <el-icon class="menu-arrow"><ArrowRight /></el-icon>
        </div>

        <div class="menu-item" @click="loadInvoices">
          <div class="menu-icon">
            <el-icon><Tickets /></el-icon>
          </div>
          <div class="menu-content">
            <span class="menu-title">我的发票</span>
            <span class="menu-desc">查看发票信息</span>
          </div>
          <el-icon class="menu-arrow"><ArrowRight /></el-icon>
        </div>
      </div>

      <!-- 发票列表 -->
      <div v-if="showInvoices" class="invoices-section">
        <div class="section-title">我的发票</div>
        <div v-loading="loadingInvoices" class="invoices-list">
          <el-empty v-if="!loadingInvoices && invoices.length === 0" description="暂无发票数据" />
          
          <div 
            v-for="invoice in invoices" 
            :key="invoice.invoiceNo"
            class="invoice-item"
          >
            <div class="invoice-info">
              <div class="invoice-no">发票号: {{ invoice.invoiceNo }}</div>
              <div class="invoice-date">开票日期: {{ invoice.invoiceDate }}</div>
              <div class="invoice-amount">金额: ¥{{ invoice.amount }}</div>
              <div class="invoice-status">
                <el-tag :type="getStatusType(invoice.status)">{{ invoice.status }}</el-tag>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 退出登录 -->
      <div class="logout-section">
        <el-button 
          type="danger" 
          plain 
          size="large"
          @click="handleLogout"
        >
          退出登录
        </el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useUserStore, useCartStore } from '@/stores'
import { customerApi } from '@/api'
import type { InvoiceOfOrderConfirmDto } from '@/types/api'

const router = useRouter()
const userStore = useUserStore()
const cartStore = useCartStore()

const showInvoices = ref(false)
const loadingInvoices = ref(false)
const invoices = ref<InvoiceOfOrderConfirmDto[]>([])

// 计算属性
const cartCount = computed(() => cartStore.cartCount)

// 加载发票列表
const loadInvoices = async () => {
  if (showInvoices.value) {
    showInvoices.value = false
    return
  }
  
  showInvoices.value = true
  loadingInvoices.value = true
  
  try {
    invoices.value = await customerApi.getInvoices()
  } catch (error) {
    console.error('加载发票失败:', error)
    ElMessage.error('加载发票失败')
    showInvoices.value = false
  } finally {
    loadingInvoices.value = false
  }
}

// 获取状态标签类型
const getStatusType = (status: string) => {
  switch (status) {
    case '已开票':
      return 'success'
    case '待开票':
      return 'warning'
    case '已作废':
      return 'danger'
    default:
      return 'info'
  }
}

// 跳转到订单页面
const goToOrders = () => {
  router.push('/orders')
}

// 跳转到购物车
const goToCart = () => {
  router.push('/cart')
}

// 返回首页
const goBack = () => {
  router.push('/products')
}

// 退出登录
const handleLogout = async () => {
  try {
    await ElMessageBox.confirm(
      '确定要退出登录吗？',
      '退出确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    userStore.logout()
    ElMessage.success('已退出登录')
    router.push('/login')
  } catch {
    // 用户取消退出
  }
}

// 页面初始化
onMounted(async () => {
  await cartStore.fetchCartCount()
})
</script>

<style scoped>
.profile-container {
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

.profile-content {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.user-info-card {
  background: white;
  border-radius: 12px;
  padding: 30px;
  display: flex;
  align-items: center;
  gap: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.user-avatar {
  flex-shrink: 0;
}

.user-details h3 {
  margin: 0 0 8px 0;
  font-size: 20px;
  color: #333;
}

.user-no {
  margin: 0;
  font-size: 14px;
  color: #666;
}

.menu-section {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 20px;
  cursor: pointer;
  transition: background-color 0.3s;
  border-bottom: 1px solid #f0f0f0;
}

.menu-item:last-child {
  border-bottom: none;
}

.menu-item:hover {
  background-color: #f8f9fa;
}

.menu-icon {
  width: 40px;
  height: 40px;
  background: #f0f7ff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #409eff;
  font-size: 18px;
  margin-right: 15px;
}

.menu-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.menu-title {
  font-size: 16px;
  color: #333;
  font-weight: 500;
}

.menu-desc {
  font-size: 14px;
  color: #666;
}

.menu-arrow {
  color: #ccc;
  font-size: 16px;
}

.invoices-section {
  background: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 15px;
  padding-bottom: 10px;
  border-bottom: 2px solid #409eff;
}

.invoices-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.invoice-item {
  padding: 15px;
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  background: #f8f9fa;
}

.invoice-info {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
  font-size: 14px;
}

.invoice-no {
  font-weight: 500;
  color: #333;
}

.invoice-date,
.invoice-amount {
  color: #666;
}

.invoice-status {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.logout-section {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

.logout-section .el-button {
  width: 200px;
  height: 50px;
  font-size: 16px;
}

@media (max-width: 768px) {
  .profile-content {
    padding: 15px;
  }

  .user-info-card {
    flex-direction: column;
    text-align: center;
    gap: 15px;
  }

  .invoice-info {
    grid-template-columns: 1fr;
    gap: 6px;
  }

  .invoice-status {
    justify-content: flex-start;
  }
}
</style>
