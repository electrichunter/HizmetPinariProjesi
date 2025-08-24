import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5186/api', // .NET projenizin çalıştığı portu buraya yazın
  withCredentials: true, 
});

export default apiClient;