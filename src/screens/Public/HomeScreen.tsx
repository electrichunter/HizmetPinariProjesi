import React, { useEffect } from 'react';
import { FlatList, StyleSheet } from 'react-native';
import { Text, Card } from 'react-native-paper';
import { ScreenWrapper, LoadingIndicator } from '@/components/common';
import { useApi } from '@/hooks/useApi';
import * as categoryApi from '@/api/categoryApi';

const HomeScreen = () => {
  const { data: categories, error, loading, request: loadCategories } = useApi(categoryApi.getCategories);

  useEffect(() => {
    loadCategories();
  }, []);

  if (loading) {
    return <LoadingIndicator />;
  }

  return (
    <ScreenWrapper>
      <Text variant="headlineMedium" style={styles.title}>Hizmetler</Text>
      {error && <Text style={styles.error}>Kategoriler yüklenemedi: {error}</Text>}
      <FlatList
        data={categories}
        keyExtractor={(item) => item.categoryID.toString()}
        renderItem={({ item }) => (
          <Card style={styles.card}>
            <Card.Title title={item.categoryName} subtitle={item.description} />
          </Card>
        )}
        showsVerticalScrollIndicator={false}
      />
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  title: {
    marginBottom: 20,
    textAlign: 'center',
  },
  card: {
    marginBottom: 15,
  },
  error: {
    color: 'red',
    textAlign: 'center',
    marginBottom: 10,
  }
});

export default HomeScreen;