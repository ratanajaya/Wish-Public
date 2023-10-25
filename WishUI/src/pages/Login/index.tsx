import { Button, Paper, Typography } from '@mui/material';
import { CredentialResponse, GoogleLogin } from '@react-oauth/google';
import { LoginResult } from 'common/types';
import useAuth from 'components/AuthProvider/useAuth';
import SpinBackdrop from 'components/SpinBackdrop';
import WishLogo from 'components/WishLogo';
import React, { useState } from 'react'
import { toast } from 'react-toastify';
import SwipeableViews from 'react-swipeable-views';
import axios from 'axios';
import appConst from 'common/appConst';

export default function Login() {
  const [loading, setLoading] = useState(false);
  const [activeStep, setActiveStep] = useState(0);
  const [temporaryToken, setTemporaryToken] = useState('');

  const { data, methods } = useAuth();

  function handleLoginAsGuest(){
    setLoading(true);

    methods.loginAsGuest(handleBackendLoginSuccess);
  }

  function handleGoogleLoginSucces(credentialResponse: CredentialResponse){
    setLoading(true);
    
    methods.loginAsGoogleAccount(credentialResponse.credential!, null, handleBackendLoginSuccess);
  }

  function handleBackendLoginSuccess(loginResult: LoginResult){
    setLoading(false);
    if(loginResult.isNew){
      setActiveStep(1);
      setTemporaryToken(loginResult.token);
    }
    else{
      reloadPage();
    }
  }

  function reloadPage(){
    window.location.href = `${process.env.REACT_APP_HOST}/#/${appConst.landingPage}`;
    window.location.reload();
  }

  const contentLogin = <>
    <GoogleLogin
      theme='filled_blue'
      size='large'
      shape='square'
      onSuccess={handleGoogleLoginSucces}
      onError={() => {
        toast.error('Google sign in failed');
      }}
      //Changed to number because it causes error since 25 july
      //@ts-ignore
      width={270} 
    />
    <div className='divider-16'></div>
    <Button variant="contained" color="primary" sx={{width:'100%'}}
      onClick={handleLoginAsGuest}
    >
      Continue as Guest
    </Button>
  </>;

  const contentAskIfUserWantToUseSample = <>
    <Button variant="contained" color="primary" sx={{width:'100%'}}
      onClick={() => {
        setLoading(true);
        axios({
          method: 'post', // or 'post', 'put', etc.
          url: `${process.env.REACT_APP_API_HOST}/Master/CreateSampleWishes`,
          headers: {
            'Authorization': `Bearer ${temporaryToken}`
          }
        })
        .then(response => {
          reloadPage();
        })
        .catch(error => {
          toast.error(JSON.stringify(error));
        });
      }}
    >
      Use Sample Wishes
    </Button>
    <div className='divider-16'></div>
    <Button variant="contained" color="primary" sx={{width:'100%'}}
      onClick={() => {
        reloadPage();
      }}
    >
      Continue With Empty Data
    </Button>
  </>;

  return (
    <div style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      height: '100vh',
      flexDirection: 'column'
    }}>
      <Paper
        elevation={6}
        sx={{
          width: '300px',
          maxWidth: '100vw',
          padding: '16px 6px 16px 6px'
        }}
      >
        <WishLogo />
        <div className='divider-16'></div>
        <SwipeableViews index={activeStep}>
          <div style={{padding:'0px 10px 0px 10px'}}>
            {contentLogin}
          </div>
          <div style={{padding:'0px 10px 0px 10px'}}>
            {contentAskIfUserWantToUseSample}
          </div>
        </SwipeableViews>
        <div className='divider-4'></div>
      </Paper>
      <div className='divider-16'></div>
      <div><Typography variant='caption'>© 2023 Ratanajaya</Typography></div>
      <SpinBackdrop open={loading} />

      {/* TODO. put this text somewhere else */}
      <div style={{display:'none'}}>
        <Typography variant='caption' paragraph align='justify'>
          Welcome to Wish - an application that helps you stay mindful of your purchases and achieve your goals! With Wish, you can track your wishes and assign scores to them every day, which will prevent impulsive buying and keep you on track.
        </Typography>
        <Typography variant='caption' paragraph align='justify'>
          You can login with Google or continue as guest.
        </Typography>
      </div>
    </div>
  )
}
