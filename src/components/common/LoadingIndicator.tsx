import React from 'react';
import { View, StyleSheet } from 'react-native';
import { ActivityIndicator, useTheme } from 'react-native-paper';

export const LoadingIndicator = () => {
  const theme = useTheme();
  return (
    <View style={styles.container}>
      <ActivityIndicator animating={true} color={theme.colors.primary} size="large" />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
});