import moment from 'moment';

const util = {
  formatErrorDisplay: (errObj: any, alt?: string) : string => {
    if(errObj?.message)
      return errObj.message;

    if(errObj?.response?.message)
      return errObj.response.message;

    if(errObj?.response?.data)
      return JSON.stringify(errObj.response.data);

    if(errObj?.response)
      return JSON.stringify(errObj.response);

    if(errObj)
      return JSON.stringify(errObj);
    
    return alt ?? 'Error. No response';
  },
  //Obsolete. Doesn't work when using hash router
  getCurrentUrlDetail: () => {
    const pathSplit = window.location.pathname.split('/');
    const urlParams = new URLSearchParams(window.location.search);

    return {
      host: window.location.host,
      path: pathSplit.length > 1 ? pathSplit[1].toLowerCase() : null,
      subPath: pathSplit.length > 2 ? pathSplit[2].toLowerCase() : null,
      id: urlParams.get('id'),
    }
  },
  isNullOrEmpty: (src: string | null | undefined) => {
    return src == null || src === '';
  },
  calculateDaysBetweenDate: (date1: string, date2: string) => { //First method written by Copilor
    const a = moment(date1);
    const b = moment(date2);
    return a.diff(b, 'days');
  },
  formatFriendlyDate: (date: string | null | undefined) => {
    if(util.isNullOrEmpty(date)) return null;

    return moment(date).format('Do of MMM');
  },
  formatFriendlyDate2: (date: string | null | undefined) => {
    if(util.isNullOrEmpty(date)) return null;

    return moment(date).format('MMM-d yyyy');
  },
};

export default util;