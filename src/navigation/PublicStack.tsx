import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import HomeScreen from '@/screens/Public/HomeScreen';
import ProfileScreen from '@/screens/Shared/ProfileScreen';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { PublicTabParamList } from '@/types/navigation';

const Tab = createBottomTabNavigator<PublicTabParamList>();

const PublicStack = () => (
  <Tab.Navigator>
    <Tab.Screen 
      name="Home" 
      component={HomeScreen} 
      options={{ 
        title: 'Ana Sayfa',
        tabBarIcon: ({ color, size }) => <MaterialCommunityIcons name="home" color={color} size={size} />
      }} 
    />
    <Tab.Screen 
      name="Profile" 
      component={ProfileScreen} 
      options={{ 
        title: 'Profil',
        tabBarIcon: ({ color, size }) => <MaterialCommunityIcons name="account" color={color} size={size} />
      }} 
    />
  </Tab.Navigator>
);

export default PublicStack;