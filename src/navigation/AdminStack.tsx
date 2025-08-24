import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import AdminDashboardScreen from '@/screens/Admin/AdminDashboardScreen';
import CategoryManagementScreen from '@/screens/Admin/CategoryManagementScreen';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { AdminTabParamList } from '@/types/navigation';

const Tab = createBottomTabNavigator<AdminTabParamList>();

const AdminStack = () => (
  <Tab.Navigator>
    <Tab.Screen 
      name="AdminDashboard" 
      component={AdminDashboardScreen} 
      options={{ 
        title: 'Admin Panel',
        tabBarIcon: ({ color, size }) => <MaterialCommunityIcons name="view-dashboard" color={color} size={size} />
      }} 
    />
    <Tab.Screen 
      name="CategoryManagement" 
      component={CategoryManagementScreen} 
      options={{ 
        title: 'Kategoriler',
        tabBarIcon: ({ color, size }) => <MaterialCommunityIcons name="shape" color={color} size={size} />
      }} 
    />
  </Tab.Navigator>
);

export default AdminStack;