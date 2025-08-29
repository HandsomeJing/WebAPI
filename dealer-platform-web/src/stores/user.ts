import { defineStore } from 'pinia'
import { ref, computed, readonly } from 'vue'
import type { User } from '@/types'
import { authApi } from '@/api'

export const useUserStore = defineStore('user', () => {
  const user = ref<User | null>(null)
  const token = ref<string>('')
  
  // 从localStorage恢复用户信息
  const initUser = () => {
    const savedToken = localStorage.getItem('token')
    const savedUser = localStorage.getItem('user')
    
    if (savedToken && savedUser) {
      token.value = savedToken
      user.value = JSON.parse(savedUser)
    }
  }
  
  // 登录
  const login = async (customerNo: string, password: string) => {
    try {
      const response = await authApi.login({ customerNo, password })
      
      token.value = response.token
      user.value = {
        customerNo: response.customerNo,
        customerName: response.customerName,
        token: response.token
      }
      
      // 保存到localStorage
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(user.value))
      
      return true
    } catch (error) {
      console.error('登录失败:', error)
      return false
    }
  }
  
  // 登出
  const logout = () => {
    user.value = null
    token.value = ''
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }
  
  // 检查是否已登录
  const isLoggedIn = computed(() => {
    return !!token.value && !!user.value
  })
  
  return {
    user: readonly(user),
    token: readonly(token),
    isLoggedIn,
    initUser,
    login,
    logout
  }
})
