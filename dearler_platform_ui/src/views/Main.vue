<template>
  <div>
    <div class="user-box">
      <div class="user-info">
        <div class="user-head">
          <img src="/img/dealerImgs/picUser.png" alt="" />
        </div>
        <p class="user-name">经销商平台</p>
  <p>销售员：陕西吴彦祖</p>
        <p>单位地址：—</p>
      </div>
    </div>

    <div class="menu-item" @click="goOrders">
      <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
        <path d="M6 2h9l3 3v15a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2z" stroke="#b70101" stroke-width="1.5" fill="#fff"/>
        <path d="M15 2v4h3" stroke="#b70101" stroke-width="1.5"/>
        <path d="M7 10h10M7 14h10M7 18h7" stroke="#b70101" stroke-width="1.5" stroke-linecap="round"/>
      </svg>
      <div class="menu-info">
        <p class="m-title">我的订单</p>
        <p class="m-info">查看订单列表</p>
      </div>
    </div>

    <div class="menu-item" @click="goCart">
      <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
        <path d="M3 4h2l2.4 10.4A2 2 0 0 0 9.36 16H18a2 2 0 0 0 1.94-1.52L22 7H6" stroke="#b70101" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        <circle cx="9.5" cy="19.5" r="1.5" fill="#b70101"/>
        <circle cx="18" cy="19.5" r="1.5" fill="#b70101"/>
      </svg>
      <div class="menu-info">
        <p class="m-title">购物车</p>
        <p class="m-info">购物车中有：{{ store.getters["shoppingCart/getCartNum"] }} 个商品</p>
      </div>
    </div>

    <div class="menu-item" @click="logout">
      <svg width="40" height="40" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" aria-hidden="true">
        <path d="M10 21H6a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" stroke="#b70101" stroke-width="1.5"/>
        <path d="M13 16l4-4-4-4" stroke="#b70101" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
        <path d="M9 12h8" stroke="#b70101" stroke-width="1.5" stroke-linecap="round"/>
      </svg>
      <div class="menu-info">
        <p class="m-title">退出帐号</p>
        <p class="m-info">退出当前帐号</p>
      </div>
    </div>
  </div>
  </template>

<script>
import { useRouter } from 'vue-router'
import { shoppingCartNum } from '../store'
import { useStore } from 'vuex'
export default {
  setup () {
    const store = useStore()
    const router = useRouter()
  const goOrders = () => router.push('/layoutMain/orderList')
    const goCart = () => router.push('/layoutMain/shoppingCart')
    const logout = () => {
      localStorage.removeItem('token')
      localStorage.removeItem('cno')
      router.push('/')
    }
    return { shoppingCartNum, store, goOrders, goCart, logout }
  }
}
</script>

<style lang="scss" scoped>
.user-box {
  padding: 10px;
  background-color: #fff;

  .user-info {
    padding: 25px 0 25px 80px;
    height: 100px;
    border-radius: 10px;
    position: relative;
    background: -webkit-linear-gradient(left, #b70101, #f20505);

    p {
      color: #fff;
      text-align: left;
      font-size: 14px;
      margin-bottom: 16px;
      color: hsla(0, 0%, 100%, 0.7);
    }

    p.user-name {
      letter-spacing: 2px;
      font-weight: bold;
      font-size: 16px;
      color: #fff;
    }

    .user-head {
      width: 40px;
      height: 40px;
      border-radius: 40px;
      border: 2px solid #fff;
      overflow: hidden;
      background-color: #fff;
      position: absolute;
      top: 36px;
      left: 20px;

      img {
        width: 40px;
        height: 40px;
      }
    }
  }
}

.menu-item {
  display: flex;
  align-items: center;
  gap: 14px;
  min-height: 72px;
  background-color: #fff;
  margin: 12px 10px 0;
  padding: 12px 16px;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  cursor: pointer;
  transition: transform .1s ease, box-shadow .2s ease;
  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px rgba(0,0,0,0.08);
  }

  img {
    width: 40px;
    height: 40px;
  }

  .menu-info {
    text-align: left;
    p.m-title {
      margin: 0;
      font-weight: 600;
      font-size: 16px;
    }
    p.m-info {
      margin: 6px 0 0;
      font-size: 12px;
      color: #666;
    }
  }
}
</style>
