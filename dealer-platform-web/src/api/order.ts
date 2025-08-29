import request from '@/utils/request'
import type { OrderMasterInputDto, SaleOrderDto, SaleOrderDetailDto, InvoiceOfOrderConfirmDto } from '@/types/api'

// 订单API接口
export const orderApi = {
  // 创建订单
  async createOrder(data: OrderMasterInputDto): Promise<boolean> {
    const response = await request.post('/api/SaleOrder', data)
    return response.data
  },

  // 获取订单列表
  async getOrders(customerNo: string): Promise<SaleOrderDto[]> {
    const response = await request.get('/api/SaleOrder', {
      params: { customerNo }
    })
    return response.data
  },

  // 获取订单详情
  async getOrderDetails(saleOrderNo: string): Promise<SaleOrderDetailDto[]> {
    const response = await request.get(`/api/SaleOrder/${saleOrderNo}/details`)
    return response.data
  },

  // 获取确认订单的发票信息
  async getOrderConfirmInvoices(customerNo: string): Promise<InvoiceOfOrderConfirmDto[]> {
    const response = await request.get('/api/Invoice/orderconfirm', {
      params: { customerNo }
    })
    return response.data
  }
}
