import { WishCrudModel, ResponseResult, Response } from 'common/types';
import { instanceWithBearer as axios } from "common/axios";

const apiMaster = {
  getWish: (params: { id: string }) => 
    axios.get<WishCrudModel>('Master/GetWish', { params }),
  getWishes: () =>
    axios.get<WishCrudModel[]>('Master/GetWishes'),
  insertWish: (param: WishCrudModel) => 
    axios.post<ResponseResult<WishCrudModel>>('Master/InsertWish', param),
  updateWish: (param: WishCrudModel) => 
    axios.post<ResponseResult<WishCrudModel>>('Master/UpdateWish', param),
  deleteWish: (params: { id: string }) => 
    axios.delete<Response>('Master/DeleteWish', { params }),
};

export default apiMaster;