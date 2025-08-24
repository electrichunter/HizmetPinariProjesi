import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { useAuth } from '@/contexts/AuthContext';
import AuthStack from '@/navigation/AuthStack';
import AdminStack from '@/navigation/AdminStack';
import SupportStack from '@/navigation/SupportStack';
import PublicStack from '@/navigation/PublicStack';
import { LoadingIndicator } from '@/components/common';
import { theme } from '@/constants/theme';

const AppNavigator = () => {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <LoadingIndicator />;
  }

  const isAdmin = user?.roles?.includes('Admin');
  const isSupport = user?.roles?.includes('Destek');

  return (
    <NavigationContainer theme={theme}>
      {user ? (
        isAdmin ? <AdminStack /> : isSupport ? <SupportStack /> : <PublicStack />
      ) : (
        <AuthStack />
      )}
    </NavigationContainer>
  );
};

export default AppNavigator;