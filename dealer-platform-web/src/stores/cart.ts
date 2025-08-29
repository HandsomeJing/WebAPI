import { defineStore } from 'pinia'
import { ref, computed, readonly } from 'vue'
import type { CartItem } from '@/types'
import { cartApi } from '@/api'

export const useCartStore = defineStore('cart', () => {
  const cartItems = ref<CartItem[]>([])
  const cartCount = ref<number>(0)
  
  // 获取购物车列表
  const fetchCartItems = async () => {
    try {
      const response = await cartApi.getCartItems('')
      cartItems.value = response.map(item => ({
        id: item.id,
        cartGuid: item.cartGuid,
        customerNo: item.customerNo,
        productNo: item.productNo,
        productNum: item.productNum,
        cartSelected: item.cartSelected,
        product: item.productDto
      }))
      await fetchCartCount()
    } catch (error) {
      console.error('获取购物车失败:', error)
    }
  }
  
  // 获取购物车数量
  const fetchCartCount = async () => {
    try {
      cartCount.value = await cartApi.getCartCount('')
    } catch (error) {
      console.error('获取购物车数量失败:', error)
    }
  }
  
  // 添加商品到购物车
  const addToCart = async (productNo: string, productNum: number = 1) => {
    try {
      await cartApi.addToCart({
        productNo,
        customerNo: '', // 在request拦截器中会自动填充
        productNum
      })
      await fetchCartItems()
      return true
    } catch (error) {
      console.error('添加到购物车失败:', error)
      return false
    }
  }
  
  // 更新购物车选中状态
  const updateCartSelected = async (cartGuid: string, cartSelected: boolean) => {
    try {
      await cartApi.updateCartSelection({ cartGuid, cartSelected })
      const item = cartItems.value.find(item => item.cartGuid === cartGuid)
      if (item) {
        item.cartSelected = cartSelected
      }
      return true
    } catch (error) {
      console.error('更新购物车状态失败:', error)
      return false
    }
  }
  
  // 获取选中的商品
  const selectedItems = computed(() => {
    return cartItems.value.filter(item => item.cartSelected)
  })
  
  // 计算总价
  const totalPrice = computed(() => {
    return selectedItems.value.reduce((total, item) => {
      const price = item.product?.salePrice || 0
      return total + (price * item.productNum)
    }, 0)
  })
  
  return {
    cartItems: readonly(cartItems),
    cartCount: readonly(cartCount),
    selectedItems,
    totalPrice,
    fetchCartItems,
    fetchCartCount,
    addToCart,
    updateCartSelected
  }
})
