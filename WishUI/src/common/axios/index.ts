import axios, { AxiosResponse } from "axios";
import { toast } from "react-toastify";
import util from "common/util";
import lsHelper from "common/lsHelper";
import appConst from "common/appConst";
import { LoginResult } from "common/types";

const loginResult = lsHelper.get<LoginResult>(appConst.key.loginResult);

const instance = axios.create({
  baseURL: `${process.env.REACT_APP_API_HOST}`,
});

const instanceWithBearer = axios.create({
  baseURL: `${process.env.REACT_APP_API_HOST}`,
  headers: {
    Authorization: `Bearer ${loginResult?.token}`,
  },
});

instanceWithBearer.interceptors.response.use(
  <T>(response: AxiosResponse<T>) => response.data, 
  errorHandler
);

instance.interceptors.response.use(
  <T>(response: AxiosResponse<T>) => response.data, 
  errorHandler
);

export { instanceWithBearer, instance };

function errorHandler(error: any) {
  console.log('errorHandler triggered', error);

  if (error?.response?.status === 401){
    setTimeout(function () {
      lsHelper.remove(appConst.key.loginResult);
      
      document.location.href = `${process.env.REACT_APP_HOST}`;
    }, 3000);

    toast.error("Session expired. Please login again");

    return Promise.reject(error);
  }

  toast.error(util.formatErrorDisplay(error));

  return Promise.reject(error);
}