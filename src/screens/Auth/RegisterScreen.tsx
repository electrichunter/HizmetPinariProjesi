import React from 'react';
import { StyleSheet } from 'react-native';
import { Text } from 'react-native-paper';
import { ScreenWrapper } from '@/components/common';

const RegisterScreen = () => (
  <ScreenWrapper style={styles.container}>
    <Text variant="titleLarge">Kayıt Ekranı</Text>
    <Text>Müşteri ve Hizmet Veren kayıt formları buraya gelecek.</Text>
  </ScreenWrapper>
);

const styles = StyleSheet.create({
  container: { justifyContent: 'center', alignItems: 'center' },
});

export default RegisterScreen;