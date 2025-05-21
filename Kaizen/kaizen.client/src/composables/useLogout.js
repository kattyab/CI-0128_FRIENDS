import { useRouter } from 'vue-router';
import axios from 'axios';

export function useLogout() {
  const router = useRouter();

  const logout = async () => {
    try {
      await axios.post('/api/login/logout', {}, { withCredentials: true })
      router.push('/auth/login');
    } catch (error) {
      console.error('Logout failed:', error);
    }
  }

  return { logout };
}
