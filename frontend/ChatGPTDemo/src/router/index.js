import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/views/AppLayout.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: AppLayout,
      children : [
        {
          path: '/home',
          name: 'Home',
          component: () => import('@/views/Home.vue')
        }
      ]
    },
    {
      path: '/auth/login',
      name: 'Login',
      component: () => import('@/views/auth/Login.vue')
    },
    {
      path: '/auth/signup',
      name: 'SignUp',
      component: () => import('@/views/auth/SignUp.vue')
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('@/views/NotFound.vue')
    },

  ]
});

export default router;
