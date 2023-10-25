import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from '@mui/material';
import React from 'react';

//NOT USED
export default function PopConfirm(props: {
  open: boolean,
  onConfirm: () => void,
  onClose: () => void
}) {
  return (
    <Dialog
      open={props.open}
      onClose={props.onClose}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">
        {"Use Google's location service?"}
      </DialogTitle>
      <DialogActions>
        <Button onClick={props.onConfirm} autoFocus>
          Confirm
        </Button>
      </DialogActions>
    </Dialog>
  )
}
