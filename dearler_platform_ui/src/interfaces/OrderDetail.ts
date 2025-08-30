export interface OrderInfo{
    order:SaleOrderDto | null
}

export interface SaleOrderDto {
    id: number;
    saleOrderNo: string;
    customerNo: string;
    invoiceNo: string;
    inputDate: string;
    stockNo: string;
    editUserNo: string;
    deliveryDate: string;
    remark: string;
    orderDetails: SaleOrderDetail[];
    orderProgress: SaleOrderProgress;
    customerInvoice: CustomerInvoice;
    // 件数合计（后端已计算好）
    totalNum: number;
    // 金额合计（后端已计算好，单位与明细一致）
    totalPrice: number;
    // 仓库显示字段（由后端根据 StockNo 填充）
    stockName: string;
    stockLinkman: string;
    stockPhone: string;
}
export interface SaleOrderDetail {
    id: number;
    saleOrderGuid: string;
    saleOrderNo: string;
    productNo: string;
    productName: string;
    productPhotoUrl: string;
    customerNo: string;
    inputDate: string;
    orderNum: number;
    basePrice: number;
    diffPrice: number;
    salePrice: number;
}
export interface SaleOrderProgress {
    id: number;
    saleOrderNo: string;
    progressGuid: string;
    stepSn: number;
    stepName: string;
    stepTime: string;
}
export interface CustomerInvoice {
    id: number;
    customerNo: string;
    invoiceNo: string;
    invoiceEin: string;
    invoiceBank: string;
    invoiceAccount: string;
    invoiceAddress: string;
    invoicePhone: string;
}