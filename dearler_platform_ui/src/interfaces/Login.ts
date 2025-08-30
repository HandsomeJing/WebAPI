export interface IloginInfo {
    userNo: string
    password: string
    login: () => Promise<void>
}