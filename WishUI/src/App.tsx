import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import 'react-toastify/dist/ReactToastify.css';
import 'styles/overrides.css';
import 'styles/app.css';

import React from 'react';
import { ToastContainer } from 'react-toastify';
import { BrowserRouter, HashRouter } from 'react-router-dom';
import SidenavLayout from 'pages/_layouts/SidenavLayout';
import { ConfirmProvider } from "material-ui-confirm";
import useAuth from 'components/AuthProvider/useAuth';
import Login from 'pages/Login';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';

function App() {
  const { data, methods } = useAuth();

  const darkTheme = createTheme({
    palette: {
      mode: 'dark',
    },
  });

  return (
    <>
      <ToastContainer
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
      />
      <ThemeProvider theme={darkTheme}>
        <CssBaseline />
        <ConfirmProvider>
          <HashRouter>
          {
            data.isAuthenticated ? <SidenavLayout /> : <Login />
          }          
          </HashRouter>
        </ConfirmProvider>
      </ThemeProvider>
    </>
  );
}

export default App;
