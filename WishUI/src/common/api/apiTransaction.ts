import { DailyWish, DailyWishInsertModel, DailyWishSummary, ResponseResult } from 'common/types';
import { instanceWithBearer as axios } from "common/axios";

const apiTransaction = {
  getDailyWishesByDate: (params: { date: string | null }) => 
    axios.get<DailyWish[]>('Transaction/GetDailyWishesByDate', { params }),
  insertDailyWish: (param: DailyWishInsertModel) => 
    axios.post<ResponseResult<DailyWish>>('Transaction/InsertDailyWish', param),
  getDailyWishSummaries: () => 
    axios.get<DailyWishSummary[]>('Transaction/GetDailyWishSummaries')
};

export default apiTransaction;