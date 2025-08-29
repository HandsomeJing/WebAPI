import request from '@/utils/request'
import type { ProductDto, BlongTypeDto, ProductTypeDto, GetProductsParams } from '@/types/api'

// 商品API接口
export const productApi = {
  // 获取商品列表
  async getProducts(params: GetProductsParams): Promise<ProductDto[]> {
    const response = await request.get('/api/Product', { params })
    return response.data
  },

  // 获取商品分类
  async getBlongTypes(): Promise<BlongTypeDto[]> {
    const response = await request.get('/api/Product/BlongType')
    return response.data
  },

  // 获取商品类型
  async getProductTypes(belongTypeName: string): Promise<ProductTypeDto[]> {
    const response = await request.get('/api/Product/type', {
      params: { belongTypeName }
    })
    return response.data
  },

  // 获取商品属性
  async getProductProps(belongTypeName: string, typeNo?: string): Promise<Record<string, string[]>> {
    const response = await request.get('/api/Product/props', {
      params: { belongTypeName, typeNo }
    })
    return response.data
  }
}
