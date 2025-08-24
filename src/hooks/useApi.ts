import { useState } from 'react';

type ApiFunc<T, P> = (params?: P) => Promise<{ data: T }>;

export const useApi = <T, P>(apiFunc: ApiFunc<T, P>) => {
  const [data, setData] = useState<T | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const request = async (params?: P) => {
    setLoading(true);
    setError(null);
    try {
      const result = await apiFunc(params);
      setData(result.data);
      return result.data;
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Beklenmedik bir hata oluştu.';
      setError(errorMessage);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { data, error, loading, request };
};