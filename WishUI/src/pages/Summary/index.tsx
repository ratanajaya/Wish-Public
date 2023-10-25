import apiTransaction from 'common/api/apiTransaction';
import { DailyWishSummary } from 'common/types';
import PageBase from 'pages/_layouts/PageBase';
import { BarChart } from '@mui/x-charts/BarChart';
import React, { useEffect, useState } from 'react';
import { Paper, styled } from '@mui/material';
import util from 'common/util';
import { HighlightScope } from '@mui/x-charts';

const StyledPaper = styled(Paper)(({ theme }) => ({
  ...theme.typography.body2,
  textAlign: 'left',
  color: theme.palette.text.secondary,
  padding:'16px 16px 16px 24px',
  marginRight: '46px'
}));

const blandFormatter = (value: number) => `${value}`;

const labelFormatter = (label: string) => {
  //label length cannot be cut because it's being used as key by BarChart
  return label; //label.length <= 6 ? label : `${label.substring(0, 5)}..`
}

export default function Summary() {
  const [loading, setLoading] = useState<boolean>(true);

  const [summaries, setSummaries] = useState<DailyWishSummary[]>([]);

  useEffect(() => {
    apiTransaction.getDailyWishSummaries()
      .then(res => {
        setSummaries(res);
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  },[]);
  
  return (
    <PageBase title='Summary' loading={loading}>
      {summaries.map(dw => {
        const dswDataSet = dw.dailySubWishes.map(dsw => ({
          name: dsw.name,
          mean: dsw.mean
        }));

        const dwDataset = [
          { name: dw.name, mean: dw.mean },
          ...dswDataSet
        ]

        const firstOrDefaultValue = dw.wishOptions.length > 0 ? dw.wishOptions[0].value : 0;

        const minOptionValue = dw.wishOptions
          .reduce((min, obj) => (obj.value < min ? obj.value : min), firstOrDefaultValue);
        const maxOptionValue = dw.wishOptions
          .reduce((max, obj) => (obj.value > max ? obj.value : max), firstOrDefaultValue);

        return(
          <div key={dw.wishId}>
            <BarChart
              dataset={dwDataset}
              yAxis={[{ 
                scaleType: 'band', 
                dataKey: 'name', 
                valueFormatter:labelFormatter 
              }]}
              xAxis={[{label:'', min:minOptionValue, max:maxOptionValue}]}
              series={[{ 
                dataKey: 'mean', 
                label: dw.name, 
                valueFormatter: blandFormatter,
                highlightScope: {
                  highlighted: 'item',
                  faded: 'global',
                } as HighlightScope
              }]}
              layout="horizontal"
              height={ 150 + (dwDataset.length * 20) }
            />
            {dw.remarks.map((a,i) => (
              <div key={i}>
                <StyledPaper elevation={3}>
                  <div 
                    style={{
                      position:'absolute',
                      zIndex:'1',
                      marginTop:'-24px',
                      marginLeft:'-8px',
                      fontSize:'12px'
                    }}
                  >
                    <span style={{fontWeight:200}}>
                      {util.formatFriendlyDate2(a.pkDate)}
                    </span>
                  </div>
                  {a.remark}
                </StyledPaper>
                <div className='divider-12' />
              </div>
            ))}
          </div>
        );
      })}
    </PageBase>
  )
}
