import React, { useState } from 'react';
import { View, StyleSheet, KeyboardAvoidingView, Platform } from 'react-native';
import { Text, Button } from 'react-native-paper';
import { useAuth } from '@/contexts/AuthContext';
import { LoginScreenProps } from '@/types/navigation';
import { StyledButton, StyledTextInput, ScreenWrapper } from '@/components/common';

const LoginScreen = ({ navigation }: LoginScreenProps) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login, isLoading } = useAuth();

  const handleLogin = async () => {
    setError('');
    if (!email || !password) {
        setError('E-posta ve şifre alanları boş bırakılamaz.');
        return;
    }
    try {
      await login(email, password);
    } catch (e) {
      setError('Giriş başarısız. Lütfen bilgilerinizi kontrol edin.');
    }
  };

  return (
    <ScreenWrapper>
      <KeyboardAvoidingView 
        behavior={Platform.OS === "ios" ? "padding" : "height"}
        style={styles.container}
      >
        <View style={styles.innerContainer}>
          <Text variant="headlineMedium" style={styles.title}>Hizmet Pınarı</Text>
          <StyledTextInput label="E-posta" value={email} onChangeText={setEmail} autoCapitalize="none" keyboardType="email-address" />
          <StyledTextInput label="Şifre" value={password} onChangeText={setPassword} secureTextEntry />
          {!!error && <Text style={styles.error}>{error}</Text>}
          <StyledButton mode="contained" onPress={handleLogin} loading={isLoading} disabled={isLoading}>
            Giriş Yap
          </StyledButton>
          <Button onPress={() => navigation.navigate('Register')}>
            Hesabın yok mu? Kayıt Ol
          </Button>
        </View>
      </KeyboardAvoidingView>
    </ScreenWrapper>
  );
};

const styles = StyleSheet.create({
  container: { flex: 1 },
  innerContainer: { flex: 1, justifyContent: 'center' },
  title: { textAlign: 'center', marginBottom: 24, fontWeight: 'bold' },
  error: { color: 'red', textAlign: 'center', marginVertical: 10 },
});

export default LoginScreen;