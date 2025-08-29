// 基础API响应接口
export interface ApiResponse<T = any> {
  success: boolean
  data: T
  message?: string
  code?: number
}

// 分页参数接口
export interface PageParams {
  pageIndex: number
  pageSize: number
}

// 排序类型枚举
export enum OrderType {
  Asc = 0,
  Desc = 1
}

// 用户登录接口
export interface LoginRequest {
  customerNo: string
  password: string
}

export interface LoginResponse {
  token: string
  customerNo: string
  customerName: string
}

// 商品相关接口
export interface ProductDto {
  id: number
  productNo: string
  productName: string
  typeName: string
  belongTypeName: string
  productPp: string
  productXh: string
  basePrice: number
  salePrice: number
  productPhotoUrl?: string
  typeNo?: string
}

export interface ProductQueryParams extends PageParams {
  searchText?: string
  productType?: string
  belongTypeName: string
  productProps?: string
  sort?: string
  orderType?: OrderType
}

// 获取商品列表的参数接口
export interface GetProductsParams {
  searchText?: string
  productType?: string
  belongTypeName: string
  productProps?: string
  sort?: string
  pageIndex?: number
  pageSize?: number
  orderType?: OrderType
}

export interface BlongTypeDto {
  typeName: string
  typeNo: string
}

export interface ProductTypeDto {
  typeNo: string
  typeName: string
  belongTypeName: string
}

// 购物车相关接口
export interface ShoppingCartInputDto {
  productNo: string
  customerNo: string
  productNum: number
}

export interface ShoppingCartDto {
  id: number
  cartGuid: string
  customerNo: string
  productNo: string
  productNum: number
  cartSelected: boolean
  productDto?: ProductDto
}

export interface ShoppingCartSelectedEditDto {
  cartGuid: string
  cartSelected: boolean
}

// 订单相关接口
export interface OrderMasterInputDto {
  deliveryDate: string
  remark?: string
}

export interface SaleOrderDto {
  id: number
  saleOrderNo: string
  customerNo: string
  invoiceNo: string
  inputDate: string
  stockNo: string
  editUserNo: string
  deliveryDate: string
  remark?: string
}

export interface SaleOrderDetailDto {
  id: number
  saleOrderGuid: string
  saleOrderNo: string
  productNo: string
  productName?: string
  productPhotoUrl?: string
  customerNo: string
  inputDate: string
  orderNum: number
  basePrice: number
  diffPrice: number
  salePrice: number
}

// 客户相关接口
export interface InvoiceOfOrderConfirmDto {
  invoiceNo: string
  invoiceDate: string
  amount: number
  status: string
}
