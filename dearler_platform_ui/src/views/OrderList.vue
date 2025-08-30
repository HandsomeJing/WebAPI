<template>
  <div class="order-list">
    <PageHead headText="我的订单" />
    <div v-if="loading" class="empty">加载中...</div>
    <div v-else-if="orders.length === 0" class="empty">暂无订单</div>
    <ul v-else>
      <li v-for="o in orders" :key="o.saleOrderNo" @click="goDetail(o.saleOrderNo)">
        <b>{{ o.saleOrderNo }}</b>
        <span> 共{{ o.totalNum }}件，合计 ¥{{ o.totalPrice.toFixed(2) }}</span>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import PageHead from '../components/PageHead.vue'
import { onMounted, reactive, toRefs } from 'vue'
import { useRouter } from 'vue-router'
import axios from '@/HttpRequests/AxiosHelper'

interface OrderLite { saleOrderNo: string; totalNum: number; totalPrice: number }
const state = reactive({ loading: true, orders: [] as OrderLite[] })
const router = useRouter()
onMounted(async ()=>{
  try{
  const res = await axios.get('/api/OrderInfo/List', { params: { take: 50 }})
  state.orders = (res.data as OrderLite[]) || []
  } finally {
    state.loading = false
  }
})
const goDetail = (orderNo: string)=> router.push({ path: '/layoutMain/orderDetail', query: { orderNo } })
const { loading, orders } = toRefs(state)
</script>

<style scoped>
.order-list { background:#fff; min-height:100vh }
.empty { padding: 40px; color:#888; text-align:center }
ul { list-style:none; margin:0; padding:0 }
li { padding:12px 16px; border-bottom:1px solid #eee; cursor:pointer }
li:hover { background:#fafafa }
</style>
