import React from 'react';
import { StyleSheet } from 'react-native';
import { Text } from 'react-native-paper';
import { useAuth } from '@/contexts/AuthContext';
import { StyledButton, ScreenWrapper } from '@/components/common';

const SupportDashboardScreen = () => {
    const { user, logout } = useAuth();
    return (
        <ScreenWrapper style={styles.container}>
            <Text variant="headlineMedium">Destek Paneline Hoş Geldin!</Text>
            <Text variant="bodyLarge" style={styles.userInfo}>Kullanıcı: {user?.fullName}</Text>
            <StyledButton mode="contained" onPress={logout}>Çıkış Yap</StyledButton>
        </ScreenWrapper>
    );
};

const styles = StyleSheet.create({
    container: { justifyContent: 'center', alignItems: 'center' },
    userInfo: { marginVertical: 20 },
});

export default SupportDashboardScreen;