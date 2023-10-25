import { Container, Typography } from '@mui/material'
import React from 'react'
import SpinBackdrop from 'components/SpinBackdrop'

export default function PageBase(props: {
  loading: boolean,
  title: string,
  children: React.ReactNode
}) {
  return (
    <div>
      <Container>
        <div className='divider-64'></div>
        <div className='divider-12'></div>
        <Typography variant='h6'>{props.title}</Typography>
        <div className='divider-12'></div>
        {props.children}
      </Container>
      <SpinBackdrop open={props.loading} />
    </div>
  )
}
