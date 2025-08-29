export interface User {
  customerNo: string
  customerName: string
  token: string
}

export interface Product {
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

export interface CartItem {
  id: number
  cartGuid: string
  customerNo: string
  productNo: string
  productNum: number
  cartSelected: boolean
  product?: Product
}

export interface ProductType {
  typeNo: string
  typeName: string
  belongTypeName: string
}

export interface ProductCategory {
  typeName: string
  typeNo: string
}
