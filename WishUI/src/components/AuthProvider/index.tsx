import React, { ReactNode, createContext, useState } from "react";

import lsHelper from "common/lsHelper";
import appConst from "common/appConst";
import { LoginResult } from "common/types";

export const AuthContext = createContext<{
  setLoginResult: (val: LoginResult) => void,
  loginResult?: LoginResult,
}>({
  setLoginResult: () => {}
});

export default function AuthProvider(props: { 
  children: ReactNode
}){
  const [loginResult, setLoginResult] = useState<LoginResult | undefined>(lsHelper.get<LoginResult>(appConst.key.loginResult));

  return (
    <AuthContext.Provider
      value={{
        loginResult: loginResult,
        setLoginResult,
      }}
    >
      {props.children}
    </AuthContext.Provider>
  );
}