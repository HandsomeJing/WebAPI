<template>
  <div class="detail">
    <div class="hero">
      <!-- 图片显示：优先使用后端返回的主图，缺失时使用占位图 -->
      <img :src="product?.productPhoto?.productPhotoUrl || fallback" alt="商品图片" />
    </div>
    <div class="info">
      <!-- 标题/分类/价格：若缺失则回退到默认展示 -->
      <h2 class="name">{{ product?.productName || '商品' }}</h2>
      <p class="type">分类：{{ product?.typeName || '-' }}</p>
      <p class="price">¥{{ transPrice(product?.productSale?.salePrice || 0) }}</p>
    </div>
    <div class="actions">
      <el-button type="primary" size="large" @click="onAddToCart">加入购物车</el-button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useRoute, useRouter } from 'vue-router'
import { reactive, toRefs } from 'vue'
import { transPrice } from '../utility/common'
import { addCart } from '../HttpRequests/ProductListRequest'
import { ElMessage } from 'element-plus'

const route = useRoute()
const router = useRouter()
// 商品详情页数据来源说明：
// 1）从商品列表跳转时，使用 router state 携带的 product 对象；
// 2）若无（直接访问详情页），页面会显示占位图与默认价格；
//    如需支持直达详情的后端拉取，可在此处根据 productNo 再发请求获取完整数据。
const state = reactive({
  product: (history.state as any)?.product || null,
  fallback: '/img/dealerImgs/picUser.png'
})

const onAddToCart = async () => {
  if (!state.product?.productNo) return
  try {
    await addCart({ customerNo: localStorage['cno'], productNo: state.product.productNo, productNum: 1 })
    ElMessage.success('已加入购物车')
    router.push('/layoutMain/shoppingCart')
  } catch { ElMessage.error('加入购物车失败') }
}

const { product, fallback } = toRefs(state)
</script>

<style scoped>
.detail { padding-bottom: 56px; }
.hero { background:#fff; text-align:center; padding:16px; }
.hero img { width: 66%; max-width: 320px; border-radius: 8px; background:#f6f6f6; }
.info { background:#fff; margin-top:12px; padding:12px 16px; text-align:left; }
.name { margin:0; font-size:18px; }
.type { margin:8px 0 0; color:#666; font-size:12px; }
.price { margin:10px 0 0; color:#f23030; font-weight:600; }
.actions { position:fixed; left:0; right:0; bottom:56px; background:#fff; padding:10px 16px; box-shadow:0 -4px 12px rgba(0,0,0,.05) }
</style>
