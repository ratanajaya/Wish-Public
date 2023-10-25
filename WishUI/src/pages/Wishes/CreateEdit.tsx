import { AddBoxOutlined, IndeterminateCheckBoxOutlined, PlusOneOutlined } from '@mui/icons-material';
import { Button, Checkbox, Divider, IconButton, TextField, Typography } from '@mui/material'
import apiMaster from 'common/api/apiMaster';
import appConst from 'common/appConst';
import { WishCrudModel, WishOptionCrudModel } from 'common/types'
import PageBase from 'pages/_layouts/PageBase';
import React, { useState } from 'react'
import { useHistory } from 'react-router-dom';
import { toast } from 'react-toastify';

export default function CreateEdit(props:{
  wish: WishCrudModel | null | undefined,
  action: 'create' | 'edit',
  onFinish: (wish: WishCrudModel) => void
}) {
  const defaultOptions : WishOptionCrudModel[] = [
    {
      id: null,
      name: 'Very Positive',
      value: 2
    },
    {
      id: null,
      name: 'Positive',
      value: 1
    },
    {
      id: null,
      name: 'Neutral',
      value: 0
    },
    {
      id: null,
      name: 'Negative',
      value: -1
    },
    {
      id: null,
      name: 'Very Negative',
      value: -2
    }
  ];

  const[wish, setWish] = useState<WishCrudModel>(props.wish ?? {
    id: null,
    name: '',
    subWishes: [],
    wishOptions: props.action === 'create' ? defaultOptions : [],
    entityState: 0
  });

  const[loading, setLoading] = useState<boolean>(false);

  function handleSave(){
    setLoading(true);

    const saveApi = props.action === 'create'
      ? apiMaster.insertWish
      : apiMaster.updateWish;

    saveApi(wish)
      .then(res => {
        if(res.success){
          toast.success(`"${wish.name}" has been saved`);
          props.onFinish(res.result!);
        }
        else{
          toast.error(res.message);
        }
      })
      .catch(() =>{})
      .finally(() => setLoading(false));
  }

  const history = useHistory();

  function handleCancel(){
    history.push('/Wishes');
  }

  if(props.action === 'edit' && props.wish == null){
    return(<div>Wish does not exist or has been deleted</div>);
  }

  return (
    <PageBase title={`${props.action === 'create' ? 'Create' : 'Edit'} Wish`} loading={loading}>
      <Divider textAlign="left">Name</Divider>
      <TextField 
        //label="Name" 
        variant="standard" 
        fullWidth={true}
        value={wish.name} 
        onChange={e => { 
          setWish(prev => {
            prev.name = e.target.value;
            return {...prev};
          })
        }}
      />
      <div className='divider-24'></div>

      <Divider textAlign="left">Subwishes</Divider>
      {wish.subWishes.map((a, i) => (
        <div key={i} style={{display:'flex', marginBottom:'12px'}}>
          <div style={{flex:'1'}}>
            <TextField 
              variant="standard" 
              fullWidth={true}
              value={a.name} 
              onChange={e => {
                setWish(prev => {
                  prev.subWishes[i].name = e.target.value;

                  return {...prev}
                })
              }}
            />
          </div>
          <div style={{width:'100px', textAlign:'end'}}>
            <Checkbox checked={a.entityState === appConst.entityState.active} 
              onChange={e => {
                setWish(prev => {
                  prev.subWishes[i].entityState = e.target.checked ? appConst.entityState.active : appConst.entityState.inactive;
                  return {...prev};
                });
              }}
            />
            <IconButton onClick={() => {
              setWish(prev => {
                prev.subWishes.splice(i, 1);
                
                return {...prev};
              })
            }}>
              <IndeterminateCheckBoxOutlined />
            </IconButton>
          </div>
        </div>
      ))}
      <div style={{width:'100%', textAlign:'end'}}>
        <IconButton onClick={() => {
          setWish(prev => {
            prev.subWishes = [
              ...prev.subWishes,
              {
                id: null,
                name: '',
                entityState: 0
              }
            ];
            
            return {...prev};
          })
        }}>
          <AddBoxOutlined />
        </IconButton>
      </div>
      <div className='divider-16'></div>

      <Divider textAlign="left">Options</Divider>
      {wish.wishOptions.map((a, i) => (
        <div key={i}>
          <div style={{display:'flex', flexDirection:'row', gap:'10px'}}>
            <div style={{width:'50px'}}>
              <TextField 
                variant="standard" 
                fullWidth={true}
                value={a.value} 
                onChange={e => {
                  setWish(prev => {
                    prev.wishOptions[i].value = parseInt(e.target.value);

                    return {...prev}
                  })
                }}
              />
            </div>
            <div style={{flex:'1'}}>
              <TextField 
                variant="standard" 
                fullWidth={true}
                value={a.name} 
                onChange={e => {
                  setWish(prev => {
                    prev.wishOptions[i].name = e.target.value;

                    return {...prev}
                  })
                }}
              />
            </div>
            <div style={{width:'50px', textAlign:'end'}}>
              <IconButton onClick={() => {
                setWish(prev => {
                  prev.wishOptions.splice(i, 1);
                  
                  return {...prev};
                })
              }}>
                <IndeterminateCheckBoxOutlined />
              </IconButton>
            </div>
          </div>
          <div className='divider-8'></div>
        </div>
      ))}
      <div style={{width:'100%', textAlign:'end'}}>
        <IconButton onClick={() => {
          setWish(prev => {
            prev.wishOptions = [
              ...prev.wishOptions,
              {
                id: null,
                value: 0,
                name: ''
              }
            ];
            
            return {...prev};
          })
        }}>
          <AddBoxOutlined />
        </IconButton>
      </div>
      <div className='divider-16'></div>
      <div style={{width:'100%', textAlign:'end'}}>
        <Button 
          variant='outlined' 
          onClick={handleCancel}
        >
          Cancel
        </Button>
        <Button 
          variant='outlined' 
          onClick={handleSave}>
          SAVE
        </Button>
      </div>
      <div className='divider-16'></div>
    </PageBase>
  )
}
