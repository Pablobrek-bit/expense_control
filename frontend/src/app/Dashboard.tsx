import { useEffect, useState } from 'react';
import { summaryService } from '../services/summary';
import type { SummaryResponse } from '../types';
import { ArrowDownCircle, ArrowUpCircle, Wallet, Loader2 } from 'lucide-react';

export default function Dashboard() {
  const [summary, setSummary] = useState<SummaryResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function loadSummary() {
      try {
        const data = await summaryService.getSummary();
        setSummary(data);
      } catch (err) {
        console.error(err);
        setError('Falha ao carregar o resumo financeiro.');
      } finally {
        setLoading(false);
      }
    }
    loadSummary();
  }, []);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(value);
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Loader2 className="animate-spin text-emerald-500" size={48} />
      </div>
    );
  }

  if (error || !summary) {
    return (
      <div className="bg-red-50 text-red-600 p-4 rounded-lg border border-red-200">
        {error}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold text-gray-800">Visão Geral</h2>
      
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 flex items-center space-x-4">
          <div className="p-3 bg-emerald-100 text-emerald-600 rounded-full">
            <ArrowUpCircle size={32} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Receitas Totais</p>
            <p className="text-2xl font-bold text-gray-900">{formatCurrency(summary.totalIncome)}</p>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 flex items-center space-x-4">
          <div className="p-3 bg-red-100 text-red-600 rounded-full">
            <ArrowDownCircle size={32} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Despesas Totais</p>
            <p className="text-2xl font-bold text-gray-900">{formatCurrency(summary.totalExpenses)}</p>
          </div>
        </div>

        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 flex items-center space-x-4">
          <div className={`p-3 rounded-full ${summary.netBalance >= 0 ? 'bg-blue-100 text-blue-600' : 'bg-orange-100 text-orange-600'}`}>
            <Wallet size={32} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-500">Saldo Líquido</p>
            <p className={`text-2xl font-bold ${summary.netBalance >= 0 ? 'text-gray-900' : 'text-red-600'}`}>
              {formatCurrency(summary.netBalance)}
            </p>
          </div>
        </div>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div className="px-6 py-4 border-b border-gray-100 bg-gray-50">
          <h3 className="text-lg font-semibold text-gray-800">Saldos Individuais</h3>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead className="text-xs text-gray-500 uppercase bg-white border-b border-gray-100">
              <tr>
                <th className="px-6 py-4 font-semibold">Membro da Família</th>
                <th className="px-6 py-4 font-semibold">Receitas</th>
                <th className="px-6 py-4 font-semibold">Despesas</th>
                <th className="px-6 py-4 font-semibold text-right">Saldo Atual</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {summary.personSummaries.map((person) => (
                <tr key={person.personId} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 font-medium text-gray-900">{person.personName}</td>
                  <td className="px-6 py-4 text-emerald-600 font-medium">{formatCurrency(person.totalIncome)}</td>
                  <td className="px-6 py-4 text-red-600 font-medium">{formatCurrency(person.totalExpenses)}</td>
                  <td className="px-6 py-4 text-right">
                    <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                      person.balance >= 0 ? 'bg-emerald-100 text-emerald-700' : 'bg-red-100 text-red-700'
                    }`}>
                      {formatCurrency(person.balance)}
                    </span>
                  </td>
                </tr>
              ))}
              
              {summary.personSummaries.length === 0 && (
                <tr>
                  <td colSpan={4} className="px-6 py-8 text-center text-gray-500">
                    Nenhuma pessoa ou transação cadastrada ainda.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
