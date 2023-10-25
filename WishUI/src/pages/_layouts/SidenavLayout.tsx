import { AddReactionOutlined, BarChartOutlined, FormatListBulletedOutlined, InfoOutlined, LogoutOutlined, MenuOutlined } from '@mui/icons-material';
import { AppBar, Avatar, Box, Button, Container, Divider, Drawer, IconButton, List, ListItem, ListItemButton, ListItemIcon, ListItemText, Paper, SwipeableDrawer, Toolbar, Typography } from '@mui/material';
import { CredentialResponse, GoogleLogin } from '@react-oauth/google';
import appConst from 'common/appConst';
import routes from 'common/routes';
import useAuth from 'components/AuthProvider/useAuth';
import WishLogo from 'components/WishLogo';
import React, { Suspense, useEffect, useState } from 'react'
import { Link, Redirect, Route, Switch, useHistory } from 'react-router-dom';
import { toast } from 'react-toastify';

export default function SidenavLayout() {
  const [open, setOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  const [popperOpen, setPopperOpen] = React.useState(false);

  const { data, methods } = useAuth();

  function handleGoogleLoginSucces(credentialResponse: CredentialResponse){
    setPopperOpen(false);
    setLoading(true);

    const guestId = data.loginResult?.user.userType === 0 ? data.loginResult?.user.id : null;

    methods.loginAsGoogleAccount(credentialResponse.credential!, guestId, () => {
      setLoading(false);
    })
  }

  const history = useHistory();

  const drawerWidth = 250;

  const listItems = [
    {
      display: 'Track Mood',
      icon: <AddReactionOutlined />,
      onClick: () => {
        history.push('/TrackMood');
      }
    },
    {
      display: 'Wishes',
      icon: <FormatListBulletedOutlined />,
      onClick: () => {
        history.push('/Wishes');
      }
    },
    {
      display: 'Summary',
      icon: <BarChartOutlined />,
      onClick: () => {
        history.push('/Summary');
      }
    },
    {
      display: 'About',
      icon: <InfoOutlined />,
      onClick: () => {
        history.push('/About');
      }
    }
  ]
  
  const list = () => (
    <Box
      sx={{ width: drawerWidth }}
      role="presentation"
      onClick={() => setOpen(false)}
      onKeyDown={() => setOpen(false)}
    >
      <div style={{height:'64px', width:'100%'}}>

      </div>
      <Divider />
      <List>
        {listItems.map((item, index) => (
          <ListItem key={item.display} disablePadding>
            <ListItemButton onClick={item.onClick}>
              <ListItemIcon>
                {item.icon}
              </ListItemIcon>
              <ListItemText primary={item.display} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );
  
  const xsOnly = { xs: 'flex', sm: 'none' };

  return (
  <>
    <SwipeableDrawer
      anchor='left'
      open={open}
      onClose={() => setOpen(false)}
      onOpen={() => setOpen(true)}
    >
      {list()}
    </SwipeableDrawer>
    <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
        }}
      >
      <Container maxWidth="xl">
        <Toolbar disableGutters sx={{minHeight:'64px'}}>
          <Box sx={{ flexGrow: 1, display: { ...xsOnly } }}>
            <IconButton
              size="large"
              aria-label="account of current user"
              aria-controls="menu-appbar"
              aria-haspopup="true"
              onClick={() => setOpen(prev => !prev)}
              color="inherit"
            >
              <MenuOutlined />
            </IconButton>
          </Box>
          <WishLogo />
          <Box sx={{ flexGrow: 1 }}></Box>
          <Box sx={{ flexGrow: 0 }}>
            <IconButton sx={{ p: 0 }} 
              onClick={(e) => setPopperOpen(prev => !prev)}
            >
              <Avatar alt={data?.loginResult?.user?.name ?? "Z"} src={data.loginResult?.user.pictureUrl!}/>
            </IconButton>
          </Box>
        </Toolbar>
      </Container>
    </AppBar>
    
    <div className='user-popup' style={{visibility: (popperOpen ? 'visible' : 'hidden') }}>
      <Paper sx={{height:'100%', borderRadius:'10px', padding:'8px'}}>
        <div style={{display:'flex', gap:'8px', width:'100%'}}>
          <Avatar alt={data?.loginResult?.user?.name ?? "Z"} src={data.loginResult?.user.pictureUrl!}/>
          <div style={{flex:'1', paddingTop:'6px'}}>
            <Typography variant='body1'>{data.loginResult?.user.name}</Typography>
          </div>
        </div>
        <div className='divider-16'></div>
        {data?.loginResult?.user?.userType !== 0 ? null :
          <div style={{width:'100%', height:'42px'}}>
            <div style={{margin:'auto'}}>
              <GoogleLogin
                theme='filled_blue'
                size='medium'
                shape='rectangular'
                onSuccess={handleGoogleLoginSucces}
                onError={() => {
                  setPopperOpen(false);
                  toast.error('Google sign in failed');
                }}
              />
            </div>
            <div className='divider-16'></div>
          </div>
        }
        <div style={{width:'100%', textAlign:'left'}}>
          <Button
            size="small"
            color="inherit"
            onClick={() => methods.logout(() => {})}
            startIcon={<LogoutOutlined />}
            variant="text"
          >
            Sign Out
          </Button>
        </div>
      </Paper>
    </div>
  
    <div style={{display:'flex'}}>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
        aria-label="mailbox folders"
      >
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {list()}
        </Drawer>
      </Box>
      <div style={{flex:'1'}}>
        <Suspense fallback={<div><h1>Loading page...</h1></div>}>
          <Switch>
            {
              routes.map(r => {
                return(
                  <Route 
                    key={r.path}
                    path={r.path}
                    component={r.component}
                    exact={false}
                  />
                );
              })
            }
            <Route exact path="/">
              <Redirect to={appConst.landingPage} />
            </Route>
            {/* <Route component={(<div>NOT FOUND</div>)} /> */}
          </Switch>
        </Suspense>
      </div>
    </div>    
  </>
  )
}
