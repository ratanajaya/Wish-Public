import React, { useEffect, useState } from 'react';
import apiMaster from 'common/api/apiMaster';
import { WishCrudModel } from 'common/types';
import { Button, Card, CardActions, CardContent, Fab, IconButton, Typography, Checkbox } from '@mui/material';
import { Edit, Delete, Add } from '@mui/icons-material';
import { useConfirm } from 'material-ui-confirm';
import util from 'common/util';
import CreateEdit from './CreateEdit';
import PageBase from 'pages/_layouts/PageBase';
import { toast } from 'react-toastify';
import { useHistory, useLocation } from 'react-router-dom';
import appConst from 'common/appConst';
import useCustomLocation from 'components/useCustomLocation';

export default function Wishes() {
  const [wishes, setWishes] = useState<WishCrudModel[]>([]);

  const[loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    setLoading(true);

    apiMaster.getWishes()
      .then(res => {
        setWishes(res);
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  },[]);

  const confirm = useConfirm();

  const history = useHistory();
  //const urlDetail = util.getCurrentUrlDetail();

  const urlDetail = useCustomLocation();

  const subPage = {
      action: urlDetail.subPath,
      id: urlDetail.id
  };

  function handleCreateWish(){
    history.push('Wishes/Create');
  }

  function handleEditWish(id: string){
    history.push(`Wishes/Edit?id=${id}`);
  }

  function handleWishStateChange(id: string, newState: boolean){
    setLoading(true);

    const wish = wishes.find(a => a.id === id)!;

    const updatedWish = {
      ...wish, 
      entityState: newState ? appConst.entityState.active : appConst.entityState.inactive
    };

    apiMaster.updateWish(updatedWish)
      .then(res => {
        toast.success(`Wish state updated`);

        setWishes(prev => {
          return [...prev.filter(a => a.id !== id), updatedWish];
        });
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }

  function handleDeleteWish(id: string){
    const wishName = wishes.find(a => a.id === id)?.name;

    confirm({ description: `Permanently delete "${wishName}"`})
      .then(() => {
        setLoading(true);

        apiMaster.deleteWish({id})
          .then(res => {
            toast.success(`${wishName} deleted`);

            setWishes(prev => {
              prev = prev.filter(a => a.id !== id);

              return [...prev];
            });
          })
          .catch()
          .finally(() => setLoading(false));
      })
      .catch(() => {});
  }

  function handleCreateEditFinish(wish: WishCrudModel){
    setWishes(prev => {
      const updatedIdx = prev.findIndex(a => a.id === wish.id);

      if(updatedIdx !== -1)
        prev[updatedIdx] = wish;
      else{
        prev.push(wish);
      }

      return [...prev];
    });

    setTimeout(function(){
      history.push(`/Wishes`);
    }, 1000);
  }

  const Main = () => {
    return (
      <PageBase title='Wish list' loading={loading}>
        {wishes.map(a => (
          <Card key={a.id} sx={{ width:'100%', marginBottom:'12px' }}>
            <CardContent>
              <div style={{display:'flex'}}>
                <div style={{flex:'1'}}>
                  <Typography variant="h6" component="div">{a.name}</Typography>
                  <Typography variant="body2" component="div">
                    Has {a.subWishes.length} sub-wishes and {a.wishOptions.length} options
                  </Typography>
                </div>
                <div style={{width:'50px', textAlign:'right'}}>
                  <Checkbox 
                    checked={a.entityState === appConst.entityState.active} 
                    onChange={e => handleWishStateChange(a.id!, e.target.checked)}
                  />
                </div>
              </div>
            </CardContent>
            <CardActions>
              <div style={{margin:'auto'}}></div>
              <IconButton size="small" onClick={() => handleEditWish(a.id!)}
                disabled={a.entityState !== appConst.entityState.active}
              >
                <Edit />
              </IconButton>
              <IconButton size="small" onClick={() => handleDeleteWish(a.id!)}
              >
                <Delete />
              </IconButton>
            </CardActions>
          </Card>
        ))}
        <div style={{position:'absolute', right:'16px', bottom:'16px'}}>
          <Fab color="primary" size='medium' onClick={handleCreateWish}>
            <Add />
          </Fab>
        </div>
      </PageBase>
    )
  }

  const editedWish = wishes.find(a => a.id === subPage.id);

  const Content = subPage.action === 'create'
    ? <CreateEdit action={subPage.action} wish={null} onFinish={handleCreateEditFinish} />
    : subPage.action === 'edit' && editedWish != null
    ? <CreateEdit action={subPage.action} wish={editedWish} onFinish={handleCreateEditFinish} />
    : <Main />;

  return (
    <>
      {Content}
    </>
  )
}
