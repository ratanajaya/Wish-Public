import { Button, Card, CardContent, Slider, TextField, Typography } from '@mui/material';
import apiMaster from 'common/api/apiMaster';
import apiTransaction from 'common/api/apiTransaction';
import appConst from 'common/appConst';
import { DailyWishInsertModel } from 'common/types';
import util from 'common/util';
import PageBase from 'pages/_layouts/PageBase';
import React, { useEffect, useRef, useState } from 'react';
import { useHistory } from 'react-router-dom';
import { toast } from 'react-toastify';

export default function TrackMood() {
  const [loading, setLoading] = useState<boolean>(false);
  const [dailyWishInserts, setDailyWishInserts] = useState<DailyWishInsertModel[]>([]);
  const [pkDate, setPkDate] = useState<string>(new Date().toISOString());

  const history = useHistory();

  useEffect(() => {
    setLoading(true);

    apiMaster.getWishes()
      .then(res1 => {
        apiTransaction.getDailyWishesByDate({ date: null })
          .then(res2 => {
            const activeWishes = res1.filter(wish => wish.entityState === appConst.entityState.active);

            const newDailyWishInserts = activeWishes.map(wish => {
              const woSorted = wish.wishOptions.sort((a, b) => a.value - b.value);

              const middleIndex = Math.floor(woSorted.length / 2);
              const medianVal = woSorted.length % 2 !== 0 ? 
                woSorted[middleIndex].value : 
                (woSorted[middleIndex - 1].value + woSorted[middleIndex].value) / 2;
              const medianId = woSorted.find(wo => wo.value === medianVal)?.id!;

              const dailyWish = res2.find(dw => dw.wishId === wish.id);

              const newDwi: DailyWishInsertModel = {
                locked: dailyWish != null,
                wish: wish,

                pkDate: pkDate,
                wishId: wish.id!,
                optionId: dailyWish?.selectedOptionId ?? medianId,
                remark: dailyWish?.remark ?? null,
                subWishOptions: wish.subWishes
                  .filter(subWish => subWish.entityState === appConst.entityState.active)
                  .map(subWish => {
                    const dailySubWish = dailyWish?.dailySubWishes.find(dsw => dsw.subWishId === subWish.id);

                    return ({
                      subWish: subWish,

                      subWishId: subWish.id!,
                      optionId: dailySubWish?.selectedOption?.id ?? medianId,
                      remark: null,
                    });
                  }),
              }

              return newDwi;
            });

            setDailyWishInserts(newDailyWishInserts);
          })
          .catch(() => {})
          .finally(() => setLoading(false));
      })
      .catch(() => setLoading(false))
      .finally(() => {});
  },[]);

  function handleSave(wishId: string, index: number){
    const param = dailyWishInserts.find(dwi => dwi.wishId === wishId)!;

    setLoading(true);
    apiTransaction.insertDailyWish(param)
      .then(res => {
        if(res.success){
          setDailyWishInserts(prev => {
            const changedIndex = prev.findIndex(dwi => dwi.wishId === wishId);
            prev[changedIndex].locked = true;

            return [...prev];
          });

          toast.success('Saved!', { autoClose: 500 });
          scrollToNext(index);
        }
        else{
          toast.error(res.message);
        }
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }
  
  const itemRefs = useRef<Array<HTMLDivElement | null>>([]);

  function scrollToNext(currIndex : number) {
    if(currIndex === itemRefs.current.length - 1) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    const topOffset = itemRefs.current[currIndex + 1]!.offsetTop;
    const navbarHeight = 64;

    window.scrollTo({ top: topOffset - navbarHeight, behavior: 'smooth' });
  }

  const headerState = dailyWishInserts.length === 0
    ? 'nothing'
    : dailyWishInserts.length > 0 && dailyWishInserts.length > 0 && dailyWishInserts.every(dwi => dwi.locked)
    ? 'finished'
    : 'normal';
  
  return (
    <PageBase title='Track Mood' loading={loading}>
      {headerState === 'nothing'
        ? <>
            <Typography variant='body1'>There's nothing here <a href='/Wishes/Create' onClick={e => {
              e.preventDefault();
              history.push('/Wishes/Create');
            }}>create a new wish</a> to start using this app</Typography>
          </>
        : headerState === 'normal'
        ? <Typography variant='body1'>Today is {`${util.formatFriendlyDate(pkDate)}`}! How do you feel of your wishes?</Typography>
        : <>
            <Typography variant='body1'>🎉You have rated all of your wishes today!🎉</Typography>
            <div className='divider-4'></div>
            <Typography variant='body1'>🎉Come back again tomorrow!🎉</Typography>
            <div className='divider-8'></div>
          </>
      }
      {dailyWishInserts.map((dwi, index) => {
          const wish = dwi.wish;

          const sliderMarks = wish.wishOptions
            .sort((woa, wob) => woa.value - wob.value)
            .map(wo => ({
              value: wo.value,
              label: wo.value
            }));

          const sliderMarksSw = sliderMarks.map(sm => ({
            value: sm.value,
          }));

          const minVal = wish.wishOptions.reduce((min, wo) => {
            return wo.value < min ? wo.value : min;
          }, 0);

          const maxVal = wish.wishOptions.reduce((max, wo) => {
            return wo.value > max ? wo.value : max;
          }, 0);
          
          const dailyWishOptionValue = wish.wishOptions.find(wo => wo.id === dwi.optionId)?.value ?? 0;

          return(
            <Card key={wish.id} 
              sx={{ width:'100%', marginBottom:'12px' }}
              ref={el => itemRefs.current[index] = el} 
            >
              <CardContent>
                <Typography variant='h6'>{wish.name}</Typography>
                <div style={{padding:'0px 8px'}}>                  
                  <Slider
                    value={dailyWishOptionValue}
                    valueLabelFormat={(val, i) =>{
                      const wishOption = wish.wishOptions.find(wo => wo.value === val);
                      return wishOption ? wishOption.name : '';
                    }}
                    step={null}
                    valueLabelDisplay="auto"
                    min={minVal}
                    max={maxVal}
                    marks={sliderMarks}
                    sx={{ marginLeft:'8px'}}
                    disabled={dwi.locked}

                    onChange={(e, val) => {
                      setDailyWishInserts(prev => {
                        const optionId = wish.wishOptions.find(wo => wo.value === val)?.id!;

                        const changedIndex = prev.findIndex(dwi => dwi.wishId === wish.id);
                        prev[changedIndex].optionId = optionId;

                        return [...prev];
                      })
                    }}
                  />
                  {dwi.subWishOptions.map(subWish =>{
                    const dailySubwishOptionValue = wish.wishOptions.find(wo => wo.id === subWish.optionId)?.value ?? 0;

                    return(
                      <div key={subWish.subWishId}>
                        <Typography variant='subtitle1'>{subWish.subWish.name}</Typography>
                        <Slider 
                          value={dailySubwishOptionValue}
                          valueLabelFormat={(val, i) =>{
                            const wishOption = wish.wishOptions.find(wo => wo.value === val);
                            return wishOption ? wishOption.name : '';
                          }}
                          step={null}
                          valueLabelDisplay="auto"
                          min={minVal}
                          max={maxVal}
                          marks={sliderMarksSw}
                          sx={{ marginLeft:'8px'}}
                          disabled={dwi.locked}

                          onChange={(e, val) => {
                            setDailyWishInserts(prev => {
                              const optionId = wish.wishOptions.find(wo => wo.value === val)?.id!;

                              const changedIndex = prev.findIndex(dwi => dwi.wishId === wish.id);
                              const changedSubIndex = prev[changedIndex].subWishOptions.findIndex(swo => swo.subWishId === subWish.subWishId);
                              prev[changedIndex].subWishOptions[changedSubIndex].optionId = optionId;

                              return [...prev];
                            });
                          }}
                        />
                      </div>
                    );
                  })}
                </div>
                <div className='divider-8'></div>
                <div style={{paddingLeft:'8px'}}>
                  <TextField 
                    multiline={true}
                    label='Remark'
                    value={dwi.remark ?? ''}
                    disabled={dwi.locked}
                    onChange={e => {
                      setDailyWishInserts(prev => {
                        const changedIndex = prev.findIndex(dwi => dwi.wishId === wish.id);
                        prev[changedIndex].remark = e.target.value;

                        return [...prev];
                      });
                    }}
                    sx={{
                      width:'100%'
                    }}
                  />
                </div>
                {!dwi.locked && (<>
                    <div className='divider-8' />
                    <div style={{textAlign:'right', marginTop:'8px'}}>
                      <Button variant='outlined' color='primary' disabled={dwi.locked}
                        onClick={() => handleSave(dwi.wishId, index)}
                      >
                        Save
                      </Button>
                    </div>
                  </>)
                }
                
              </CardContent>
            </Card>
          );
        }
      )}
    </PageBase>
  )
}
