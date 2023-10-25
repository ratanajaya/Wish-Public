const lsHelper = {
  get: <T>(key: string) => {
    const serializedValue = localStorage.getItem(key);
    if (serializedValue == null) {
      return undefined;
    }
    return JSON.parse(serializedValue) as T;
  },
  
  remove: (key: string) => {
    localStorage.removeItem(key);
  },
  
  set: (key: string, value: any) => {
    const serializedValue = JSON.stringify(value);
    localStorage.setItem(key, serializedValue);
  }
};

export default lsHelper;