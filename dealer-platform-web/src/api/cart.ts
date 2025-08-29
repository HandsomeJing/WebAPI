import request from '@/utils/request'
import type { ShoppingCartDto, ShoppingCartInputDto, ShoppingCartSelectedEditDto } from '@/types/api'

// 购物车API接口
export const cartApi = {
  // 获取购物车列表
  async getCartItems(customerNo: string): Promise<ShoppingCartDto[]> {
    const response = await request.get('/api/ShoppingCart', {
      params: { customerNo }
    })
    return response.data
  },

  // 添加到购物车
  async addToCart(data: ShoppingCartInputDto): Promise<boolean> {
    const response = await request.post('/api/ShoppingCart', data)
    return response.data
  },

  // 更新购物车商品数量
  async updateCartQuantity(cartGuid: string, productNum: number): Promise<boolean> {
    const response = await request.put(`/api/ShoppingCart/${cartGuid}`, {
      productNum
    })
    return response.data
  },

  // 更新购物车选中状态
  async updateCartSelection(data: ShoppingCartSelectedEditDto): Promise<boolean> {
    const response = await request.put('/api/ShoppingCart/selected', data)
    return response.data
  },

  // 删除购物车商品
  async removeFromCart(cartGuid: string): Promise<boolean> {
    const response = await request.delete(`/api/ShoppingCart/${cartGuid}`)
    return response.data
  },

  // 获取购物车商品数量
  async getCartCount(customerNo: string): Promise<number> {
    const response = await request.get('/api/ShoppingCart/count', {
      params: { customerNo }
    })
    return response.data
  }
}
