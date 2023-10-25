import { WishCrudModel, ResponseResult, Response, LoginResult } from 'common/types';
import { instanceWithBearer as axios } from "common/axios";

const apiAuth = {
  loginAsGuest: () => 
    axios.post<ResponseResult<LoginResult>>('Auth/LoginAsGuest'),
  loginAsGoogleAccount: (idToken: string, guestId: string | null) =>
    axios.post<ResponseResult<LoginResult>>(`Auth/LoginAsGoogleAccount`, null, { params: { idToken, guestId } }),
};

export default apiAuth;