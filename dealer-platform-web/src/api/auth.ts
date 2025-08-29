import request from '@/utils/request'
import type { LoginRequest, LoginResponse } from '@/types/api'

// 认证API接口
export const authApi = {
  // 用户登录
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await request.post('/api/Auth/login', data)
    return response.data
  },

  // 刷新token（如果后端支持）
  async refreshToken(): Promise<LoginResponse> {
    const response = await request.post('/api/Auth/refresh')
    return response.data
  }
}
