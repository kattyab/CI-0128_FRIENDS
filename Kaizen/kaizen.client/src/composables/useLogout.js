import { useRouter } from 'vue-router';
import axios from 'axios';

export function useLogout() {
  const router = useRouter();

  const logout = async () => {
    try {
      await axios.post(`${import.meta.env.VITE_API_URL}/api/login/logout`, {}, { withCredentials: true })
      localStorage.removeItem('companyPk');
      router.push('/auth/login');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  }

  return { logout };
}
