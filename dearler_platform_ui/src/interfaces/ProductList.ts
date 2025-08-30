export interface IProductInputDto {
    searchText: string | null,
    belongTypeName: string,
    productType: string | null,
    productProps: string | null,
    sort: string,
    pageIndex: number,
}

export interface IProductPropInputDto {
    belongTypeName: string,
    typeNo: string | null
}

export interface IProductInfo {
    systemIndex: string,
    searchText: string,
    propSelect: any,
    products: IProduct[],
    loadingProducts: boolean,
    timer: number | null;
    belogTypes: IBlongType[],
    typeSelected: string | null,
    productTypes: IProductType[],
    productProps: any,
    pageIndex:number,
    getPruducts: (belongTypeName: string, productType: string | null, searchText: string | null, productProps: string | null) => void;
    getBelongTypes: () => void,
    getType: (belongTypeName: string) => void;
    getProps: (belongTypeName: string, typeNo: string | null) => void;
    onAddCart: (productNo: string, productNum: number) => Promise<void>;
    confirmFilter: () => void;
    search: () => void;
    selectProp: (propKey: string, propValue: string) => void;
}
export interface IProduct {
    id: number;
    sysNo: string;
    productNo: string;
    productName: string;
    typeNo: string;
    typeName: string;
    productPp: string;
    productXh: string;
    productCz: string;
    productHb: string;
    productHd: string;
    productGy: string;
    productHs: string;
    productMc: string;
    productDj: string;
    productCd: string;
    productGg: string;
    productYs: string;
    unitNo: string;
    unitName: string;
    productNote: string;
    productBzgg: string;
    belongTypeNo: string;
    belongTypeName: string;
    productPhoto: IProductPhoto;
    productSale: IProductSale;
}
export interface IProductPhoto {
    id: number;
    sysNo: string;
    productNo: string;
    productPhotoUrl: string;
}
export interface IProductSale {
    id: number;
    sysNo: string;
    productNo: string;
    stockNo: string;
    salePrice: number;
}
export interface IBlongType {
    sysNo: string;
    belongTypeName: string;
}
export interface IProductType {
    id?: number;
    typeNo: string;
    typeName: string;
}

export interface IShoppingCartInputDto{
    customerNo:string;
    productNo:string;
    productNum:number
}