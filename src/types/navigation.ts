import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import type { BottomTabScreenProps } from '@react-navigation/bottom-tabs';

export type AuthStackParamList = {
  Login: undefined;
  Register: undefined;
};

export type AdminTabParamList = {
  AdminDashboard: undefined;
  CategoryManagement: undefined;
};

export type SupportTabParamList = {
    SupportDashboard: undefined;
};

export type PublicTabParamList = {
    Home: undefined;
    Profile: undefined;
};

export type LoginScreenProps = NativeStackScreenProps<AuthStackParamList, 'Login'>;