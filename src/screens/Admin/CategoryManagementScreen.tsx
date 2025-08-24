import React from 'react';
import { StyleSheet } from 'react-native';
import { Text } from 'react-native-paper';
import { ScreenWrapper } from '@/components/common';

const CategoryManagementScreen = () => (
  <ScreenWrapper style={styles.container}>
    <Text variant="titleLarge">Kategori Yönetimi</Text>
    <Text>Kategori ekleme, silme, güncelleme işlemleri burada yapılacak.</Text>
  </ScreenWrapper>
);

const styles = StyleSheet.create({
  container: { justifyContent: 'center', alignItems: 'center' },
});

export default CategoryManagementScreen;