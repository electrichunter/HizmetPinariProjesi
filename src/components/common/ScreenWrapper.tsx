import React, { ReactNode } from 'react';
import { View, StyleSheet, SafeAreaView, StatusBar, Platform } from 'react-native';
import { useTheme } from 'react-native-paper';

type Props = {
  children: ReactNode;
  style?: object;
};

export const ScreenWrapper = ({ children, style }: Props) => {
  const { colors } = useTheme();

  return (
    <SafeAreaView style={[styles.safeArea, { backgroundColor: colors.background }]}>
      <StatusBar 
        barStyle={Platform.OS === 'ios' ? 'dark-content' : 'default'} 
        backgroundColor={colors.background} 
      />
      <View style={[styles.container, style]}>
        {children}
      </View>
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
  },
  container: {
    flex: 1,
    padding: 16,
  },
});