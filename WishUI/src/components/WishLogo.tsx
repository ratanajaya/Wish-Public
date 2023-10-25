import { AccountBalanceWalletOutlined } from '@mui/icons-material'
import { Typography, Box } from '@mui/material'
import { ResponsiveStyleValue, SystemStyleObject, Theme } from '@mui/system'
import { Property } from 'csstype'
import React from 'react'

export default function WishLogo(props:{
  display?: ResponsiveStyleValue<string[] | Property.Display | undefined> | SystemStyleObject<Theme>
}) {
  return (
    <Box sx={{
      display: props.display ?? 'flex',
      justifyContent: 'center',
      alignItems: 'center',
    }}>
      <AccountBalanceWalletOutlined sx={{ mr: 1 }} />
      <Typography
        variant="h5"
        noWrap
        component="a"
        href="/"
        sx={{
          fontFamily: 'monospace',
          fontWeight: 700,
          letterSpacing: '.3rem',
          color: 'inherit',
          textDecoration: 'none',
        }}
      >
        WISH
      </Typography>
    </Box>
  )
}
