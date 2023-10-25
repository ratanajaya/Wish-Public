//This type definition overrides response type of axios instances from AxiosResponse<T> to T
//The need arises due to usage of response interceptor that returns response.data
//Source: https://github.com/axios/axios/issues/1510#issuecomment-525382535

import axios from 'axios';

declare module 'axios' {
  export interface AxiosInstance {
    request<T = any> (config: AxiosRequestConfig): Promise<T>;
    get<T = any>(url: string, config?: AxiosRequestConfig): Promise<T>;
    delete<T = any>(url: string, config?: AxiosRequestConfig): Promise<T>;
    head<T = any>(url: string, config?: AxiosRequestConfig): Promise<T>;
    post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>;
    put<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>;
    patch<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T>;
  }
}