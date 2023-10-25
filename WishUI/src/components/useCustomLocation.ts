import { useLocation } from "react-router-dom";

export default function useCustomLocation() {
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);

  const pathSplit = location.pathname.split('/');

  return {
    path: pathSplit.length > 1 ? pathSplit[1].toLowerCase() : null,
    subPath: pathSplit.length > 2 ? pathSplit[2].toLowerCase() : null,
    id: searchParams.get('id')
  };
}