import { AuthContext } from 'components/AuthProvider';

import { useCallback, useContext, useMemo } from "react";
import apiAuth from 'common/api/apiAuth';
import { toast } from 'react-toastify';
import { LoginResult } from 'common/types';
import lsHelper from 'common/lsHelper';
import appConst from 'common/appConst';
import util from 'common/util';

export default function useAuth() {
  const { loginResult, setLoginResult } = useContext(
    AuthContext
  );

  const token = useMemo(() => {
    return loginResult?.token;
  }, [loginResult]);

  const isAuthenticated = !util.isNullOrEmpty(loginResult?.token);

  const loginAsGuest = useCallback((onFinish: (lr: LoginResult) => void) => {
    apiAuth.loginAsGuest()
      .then(res => {
        if(res.success){
          lsHelper.set(appConst.key.loginResult, res.result);
          onFinish(res.result!);
        }
        else{
          toast.error(res.message);
        }
      })
      .catch()
      .finally();
  }, []);

  const loginAsGoogleAccount = useCallback((idToken: string, guestId: string | null, onFinish: (lr: LoginResult) => void) => {
    apiAuth.loginAsGoogleAccount(idToken, guestId)
      .then(res => {
        if(res.success){
          lsHelper.set(appConst.key.loginResult, res.result);
          onFinish(res.result!);
        }
        else
          toast.error(res.message);
      })
      .catch()
      .finally();
  },[]);

  const logout = useCallback(async (onFinish: () => void) => {
    lsHelper.remove(appConst.key.loginResult);
    window.location.href = `${process.env.REACT_APP_HOST}`;
    onFinish();
  }, []);

  return {
    data: {
      isAuthenticated,
      loginResult,
      token,
    },
    methods: {
      logout,
      loginAsGuest,
      loginAsGoogleAccount,
    },
  };
}