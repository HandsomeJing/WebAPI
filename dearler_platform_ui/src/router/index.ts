import { createRouter, createWebHashHistory, RouteRecordRaw } from 'vue-router'
import Home from '../views/Home.vue'
import Login from '../views/Login.vue'

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/layoutMain',
    name: 'LayoutMain',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
  component: () => import(/* webpackChunkName: "about" */ '../views/LayoutMain.vue'),
  redirect: '/layoutMain/main',
    children: [
      {
    path: 'main',
        name: 'main',
        component: () => import(/* webpackChunkName: "about" */ '../views/Main.vue'),
      },
      {
    path: 'productList',
        name: 'productList',
        component: () => import(/* webpackChunkName: "about" */ '../views/ProductList.vue'),
      },
  // 按业务要求：移除“商品详情”页面（此前为新增功能）
      {
    path: 'shoppingCart',
        name: 'shoppingCart',
        component: () => import(/* webpackChunkName: "about" */ '../views/ShoppingCart.vue'),
      },
      {
    path: 'orderConfirm',
        name: 'orderConfirm',
        component: () => import(/* webpackChunkName: "about" */ '../views/OrderConfirm.vue'),
      },
      {
    path: 'orderDetail',
        name: 'orderDetail',
        component: () => import(/* webpackChunkName: "about" */ '../views/OrderDetail.vue'),
      },
    ]
  },
  {
    path: '/about',
    name: 'About',
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () => import('../views/About.vue')
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes
})

// 简单登录守卫：无 token 则仅允许访问登录页
router.beforeEach((to, from, next) => {
  const allowList = ['/', '/about']
  const hasToken = !!localStorage.getItem('token')
  if (hasToken || allowList.includes(to.path)) {
    next()
  } else {
    next('/')
  }
})

export default router
