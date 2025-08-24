import React from 'react';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import SupportDashboardScreen from '@/screens/Support/SupportDashboardScreen';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { SupportTabParamList } from '@/types/navigation';

const Tab = createBottomTabNavigator<SupportTabParamList>();

const SupportStack = () => (
  <Tab.Navigator>
    <Tab.Screen 
      name="SupportDashboard" 
      component={SupportDashboardScreen} 
      options={{ 
        title: 'Destek Panel',
        tabBarIcon: ({ color, size }) => <MaterialCommunityIcons name="face-agent" color={color} size={size} />
      }} 
    />
  </Tab.Navigator>
);

export default SupportStack;